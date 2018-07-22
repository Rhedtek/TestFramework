using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using AutomationTestServer.DataObjects;
using AutomationTestServer.DatabaseObjects;
using AutomationTestPowerCLI;
using AutomationTestCommon;

namespace AutomationTestServer
{
    internal class VMInstance
    {
        private enum VMInstanceState
        {
            Unknown,
            Initializing,
            Initialized
        };

        private ConnectionHandler mConnectionHandler = null;
        private bool mVMInstanceBusy = false;   // This flag is set to true when the VM itself is handling a command
        private bool mBusy = false;
        private VMInstanceData mData;
        private bool mTesting = false;
        private Timer mInactivityTimer = null;  // resets the VM after x minutes of inactivity
        private string mDefaultSnapshot = "Service";
        private static string disk_l = File.Exists("E:\\") ? "E:\\" : "C:\\";
        private string mOutputDirectory = disk_l + "Output";
        private string mDumpDirectory = disk_l + "Dumps";
        private NetworkCredential mNetworkCredentials;
        private Logger mLogFile = null;
        private Process mChildProcess = null;
        private bool mCreateSnapshotWhenCrashed = false;

        // Heartbeats
        private int mHeartbeatsMissed = 0;
        private Timer mHeartbeatTimer = null;
        private const int mHeartbeatTimerTimeoutMs = 90000;  // 1.5 minutes
        private const int mMaxHeartBeatMissed = 3;

        internal VMInstance(VMInstanceData data, TestServer testServer, NetworkCredential networkCredentials)
        {
            mData = data;
            mNetworkCredentials = networkCredentials;
            StartLog();
            StartHeartbeatTimer();
        }

        internal void Start(TcpClient client)
        {
            Log("Handling new connection, CurrentConnection=" + (mConnectionHandler != null) + ", Busy=" + mBusy);

            // Apparently it is active
            mData.State = 1;

            // Reset missed heart beats
            mHeartbeatsMissed = 0;

            if (mConnectionHandler != null || mBusy == true)
            {
                Log("Closing connection to client.");

                // Another connection while we have a connection open already. This happens during snapshot manipulation and is OK.
                client.Close();
            }
            else
            {
                // Handle the new connection
                mConnectionHandler = new ConnectionHandler(client, this);
                mConnectionHandler.Start();
            }
        }

        internal bool Busy()
        {
            return (mConnectionHandler != null);
        }

        internal StartClientResponse HandleStartMessage()
        {
            Log("Start message received.");

            // Get latest state
            mData = VMInstanceData.SelectByIP(mData.IPAddress);

            // Reset local flags that represent the state of the VM
            mBusy = false;

            StartClientResponse response = new StartClientResponse()
            {
                Enabled = (mData.State == 1),
                OSType = mData.OperatingSystemID
            };

            Log("Start response: Enabled=" + response.Enabled + "; OS=" + response.OSType);
            return response;
        }

        internal InitialMessageResponse HandleInitialMessage()
        {
            Log("Initial message received.");
            mData.Start();

            if (mChildProcess != null)
            {
                Log("Killing child process with Id " + mChildProcess.Id);
                try
                {
                    mChildProcess.Kill();
                }
                catch (Exception ex)
                {
                    Log("Exception: " + ex);
                }
                mChildProcess = null;
            }

            return new InitialMessageResponse() { Now = DateTime.Now };
        }

        internal Tuple<TestJobCommandData, string> HandleGetCommand(ATCommandRequest request)
        {
            Log("HandleGetCommand, Busy: " + mBusy + ", VM Busy: " + mVMInstanceBusy + ", ServiceState=" + request.ServiceState);

            var command = TestJobCommandData.GetNextCommand(mData.VMInstanceID);
            if (command == null)
            {
                Log("Requesting next command for VMInstanceID " + mData.VMInstanceID + ": no command was returned.");
                return null;
            }

            Log("Next command for VMInstanceID " + mData.VMInstanceID + ": '" + command.TestCommandString + "' with ID " + command.TestCommandID +
                ", ExecutionOrder " + command.ExecutionOrder + ", MaxExecutionOrder " + command.MaxExecutionOrder);

            var result = HandleTestJobCommand(command); // Tuple.Item1: handled locally; Item2: result of local action
            if (result.Item1 == true)
            {
                // upload results
                TestResults testResults = new TestResults()
                {
                    Passed = (result.Item2 == true ? 1 : 0),
                    Warnings = 0,
                    Failed = (result.Item2 == false ? 1 : 0),
                    Skipped = 0
                };
                var testJobState = command.MarkAsDone(mData.VMInstanceID, testResults, false, "Handled by TestServer.", string.Empty, string.Empty);

                if (command.MaxExecutionOrder == command.ExecutionOrder || testJobState > 4)
                {
                    Log("End of TestJob reached; resetting the VM. TestJobState " + testJobState);
                    SendEmail(command.TestJobID, command.UserName);
                    CleanupVM();
                }

                // get ready for the next command request
                command = null;
                return null;
            }
            else
            {
                // Command needs to be handled by the VM
                StartInactivityTimer(command);
                mVMInstanceBusy = true;
                return Tuple.Create(command, "N/A");
            }
        }

        internal HeartbeatResponse HandleHeartbeat(HeartbeatRequest request)
        {
            Log("HandleHeartbeat from " + mData.VMName + ", TestCommandID=" + request.TestCommandID + ", RunCount=" + request.RunCount);

            lock (this)
            {
                mHeartbeatsMissed = 0;
            }

            bool isRunning = true;
            if (request != null && request.TestCommandID != -1)
            {
                isRunning = TestJobCommandData.IsRunning(request.TestCommandID, request.RunCount);
            }

            return new HeartbeatResponse() { TestRunning = isRunning };
        }

        internal void HandleCommandDone(ATCommandDoneRequest indication)
        {
            int testJobState = -1;

            Log("HandleCommandDone, TestCommandID=" + indication.TestCommandID + ", Result=" + indication.Result + ", ResultString=" + indication.ResultString);

            StopInactivityTimer();

            try
            {
                var result = CopyFilesLocally(indication);
                if (result == true)
                {
                    var testOutput = ParseFilesLocally(indication);
                    if (testOutput.Results.Failed < 0)
                    {
                        testOutput.Results.Failed = Math.Abs(testOutput.Results.Failed);
                    }

                    // Temp fix. The real fix should have a Boolean flag in the CommandDoneRequest that indicates that the command has timed out.
#warning todo
                    if (indication.TimedOut)
                    {
                        testOutput.Results.Failed = 1;

                    }
                    else
                    {
                        // Delete db files
                        DeleteShareFiles(indication);
                    }
                    /*
                    if (indication.ResultString.Contains("timed out"))
                    {
                        testOutput.Results.Failed = 1;
                    }

                    if (testOutput.Results.Failed == 0 && testOutput.Results.Warnings == 0)
                    {
                        // Delete db files
                        DeleteShareFiles(indication);
                    }
                    */
                    testJobState = TestJobCommandData.MarkAsDone(mData.VMInstanceID, indication.TestCommandID, testOutput.Results,
                        (testOutput.NumberOfDumpFiles > 0), indication.ResultString, testOutput.Version, testOutput.DownloadLink);
                }
                else
                {
                    TestResults testResults = new TestResults()
                    {
                        Passed = 0,
                        Warnings = 1,
                        Failed = 0,
                        Skipped = 0
                    };
                    testJobState = TestJobCommandData.MarkAsDone(mData.VMInstanceID, indication.TestCommandID, testResults, false, "Could not copy files locally.", indication.ResultString, string.Empty);
                }
            }
            catch (Exception ex)
            {
                Log("Exception: " + ex);
                TestResults testResults = new TestResults()
                {
                    Passed = 0,
                    Warnings = 1,
                    Failed = 0,
                    Skipped = 0
                };
                testJobState = TestJobCommandData.MarkAsDone(mData.VMInstanceID, indication.TestCommandID, testResults, false, indication.ResultString, string.Empty, string.Empty);
            }

            // End of the test job when all commands have been run, or when the state is 5 (Cancelled) or 6 (Deleted)
            if (indication.MaxExecutionOrder == indication.ExecutionOrder || testJobState > 4)
            {
                Log("End of TestJob reached; resetting the VM. TestJobState " + testJobState, true);
                SendEmail(indication.TestJobID, indication.UserName);
                CleanupVM();
            }
            else if (testJobState == 4)     // Paused
            {
                // Create snapshot
                string tempSnapshotName = "PAUSED_" + indication.TestJobID;
                RemoveSnapshot(tempSnapshotName);
                CreateSnapshot(tempSnapshotName);
            }

            // get ready for the next command request
            mVMInstanceBusy = false;
        }

        internal void HandleConnectionDone()
        {
            CloseConnection();
        }

        internal string VMName
        {
            get
            {
                return mData.VMName;
            }
        }

        internal string IPAddress
        {
            get
            {
                return mData.IPAddress;
            }
        }

        internal void Log(string msg, bool stdOut = false)
        {
            msg = "[" + DateTime.Now + "] " + msg;

            if (stdOut == true)
            {
                Console.WriteLine("[" + mData.VMName + "] " + msg);
            }

            mLogFile.Log(msg);
        }

        private bool Initialize(string snapshot)
        {
            bool result = true;

            if (mTesting == false)
            {
                mBusy = true;

                // Refresh data in case the VM name has changed.....
                mData = VMInstanceData.SelectByIP(mData.IPAddress);

                // Stop the heartbeat timer because rolling it back might cause it to actually miss a heartbeat.
                StopHeartbeatTimer();
                PowerCLI powerCLI = PowerCLI.Instance(mNetworkCredentials);
                result = powerCLI.RollbackVM(mData.VMName, snapshot, mLogFile);
                if (result == false)
                {
                    DatabaseLog.Insert("Rollback failed on VM " + mData.VMName, "There might be an issue with PowerCLI. Please check the logs.", 1);
                }
                StartHeartbeatTimer();
                mBusy = false;
            }

            return result;
        }

        private bool CreateSnapshot(string snapshot)
        {
            if (snapshot.ToLower() == "service")
            {
                Log("Not allowed to create Service snapshot.", true);
                return false;
            }

            bool result = true;

            if (mTesting == false)
            {
                Log("Creating snapshot " + snapshot, true);
                StopHeartbeatTimer();
                PowerCLI powerCLI = PowerCLI.Instance(mNetworkCredentials);
                result = powerCLI.CreateSnapshot(mData.VMName, snapshot, mLogFile);
                if (result == false)
                {
                    DatabaseLog.Insert("CreateSnapshot failed on VM " + mData.VMName, "There might be an issue with PowerCLI. Please check the logs.", 1);
                }
                StartHeartbeatTimer();
            }

            return result;
        }

        private bool RemoveSnapshot(string snapshot)
        {
            if (mTesting == false)
            {
                mBusy = true;
                Log("Removing snapshot " + snapshot);
                StopHeartbeatTimer();
                PowerCLI powerCLI = PowerCLI.Instance(mNetworkCredentials);
                powerCLI.RemoveSnapshot(mData.VMName, snapshot, mLogFile);
                StartHeartbeatTimer();
                mBusy = false;
            }

            return true;
        }

        private bool RebootVM()
        {
            bool result = true;
            if (mTesting == false)
            {
                // Refresh data in case the VM name has changed.....
                mData = VMInstanceData.SelectByIP(mData.IPAddress);

                Log("Rebooting VM " + mData.VMName);
                StopHeartbeatTimer();
                PowerCLI powerCLI = PowerCLI.Instance(mNetworkCredentials);
                result = powerCLI.Reboot(mData.VMName, mLogFile);
                if (result == false)
                {
                    DatabaseLog.Insert("Rebooting failed for VM " + mData.VMName, "There might be an issue with PowerCLI. Please check the logs.", 1);
                }
                StartHeartbeatTimer();
            }

            return result;
        }

        // If true is returned, the command has been handled
        private Tuple<bool, bool> HandleTestJobCommand(TestJobCommandData testJobCommand)
        {
            bool handledLocally;
            bool handledSuccessfully = false;

            string[] results = testJobCommand.TestCommandString.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            var command = results[0].ToLower();

            if (command == "installapp")
            {
                handledLocally = false;
            }
            else if (command == "entrypoint")
            {
                handledLocally = false;
            }
            else if (command == "rollback")
            {
                handledLocally = true;
                if (results.Length == 2)
                {
                    handledSuccessfully = Initialize(results[1]);
                }
                else
                {
                    handledSuccessfully = Initialize(mDefaultSnapshot);
                }
            }
            else if (command == "createsnapshot")
            {
                handledLocally = true;
                mBusy = true;
                handledSuccessfully = RemoveSnapshot(results[1]);
                handledSuccessfully = handledSuccessfully && CreateSnapshot(results[1]);
                mBusy = false;
            }
            else if (command == "stoponerror")
            {
                handledLocally = true;
                handledSuccessfully = false;

                bool result = false;
                if (bool.TryParse(results[1], out result) == true)
                {
                    try
                    {
                        TestJobData.SetStopOnFailure(testJobCommand.TestJobID, result);
                        handledSuccessfully = true;
                    }
                    catch (Exception ex)
                    {
                        Log("Exception: " + ex);
                    }
                }
                else
                {
                    Log("Invalid command received: " + testJobCommand.TestCommandString);
                }
            }
            else if (command == "reboot")
            {
                handledLocally = true;
                handledSuccessfully = RebootVM();

                StopHeartbeatTimer();

                for (int index = 0; index < 3; ++index)
                {
                    var pingResult = Ping();
                    if (pingResult == true)
                        break;
                }
                ConnectRDP();

                StartHeartbeatTimer();
            }
            else if (command == "timeout")
            {
                handledLocally = true;
                int timeout = int.Parse(results[1]);    // package uploader has verified that it is a valid int and value
                TestJobData.SetTimeout(testJobCommand.TestJobID, timeout);
                handledSuccessfully = true;
            }
            else
            {
                handledLocally = true;
                handledSuccessfully = false;

                Log("Invalid command received: " + testJobCommand.TestCommandString);
            }

            return new Tuple<bool, bool>(handledLocally, handledSuccessfully);
        }

        private void CloseConnection()
        {
            if (mConnectionHandler != null)
            {
                mConnectionHandler.Stop();
                mConnectionHandler = null;
            }
        }

        private void StartInactivityTimer(TestJobCommandData testJobCommand)
        {
            if (mInactivityTimer != null)
            {
                Log("Inactivity timer is not NULL.");
                StopInactivityTimer();
            }

            mInactivityTimer = new Timer(InactivityTimeout, null, (testJobCommand.TimeoutMinutes + 5) * 60 * 1000, 0);
        }

        private void StopInactivityTimer()
        {
            if (mInactivityTimer != null)
            {
                mInactivityTimer.Dispose();
                mInactivityTimer = null;
            }
        }

        private void InactivityTimeout(object state)
        {
            // Error situation
            Log("Error: inactivity timer expired. Reseting VM.", true);

            mData = VMInstanceData.SelectByIP(mData.IPAddress);
            Log("Fetched updated information for VM. VMState: " + mData.State);
            if (mData.State == 1)
            {
                CleanupVM();
            }
        }

        private void CleanupVM()
        {
            CloseConnection();
            StopInactivityTimer();
            mVMInstanceBusy = false;
            Initialize(mDefaultSnapshot);
        }

        private bool CopyFilesLocally(ATCommandDoneRequest indication)
        {
            bool result = false;
            string directory = GetOutputDirectory(indication.TestJobID, indication.TestCommandID);
            try
            {
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }
                else
                {
                    Log("Deleting existing directory " + directory);
                    try
                    {
                        Directory.Delete(directory, true);
                    }
                    catch (Exception ex)
                    {
                        Log("Exception when deleting directory: " + ex.Message);
                    }
                }

                string shareDirectory = "\\\\" + mData.HostName;
                if (mData.HostName.ToLower().Contains("APPmac") == true)
                {
                    shareDirectory = "\\\\" + mData.IPAddress + "\\testuser";
                    PinvokeWindowsNetworking.connectToRemote(@"\\" + mData.IPAddress + @"\testuser\Share", "testuser", "password");
                }
                shareDirectory = shareDirectory + "\\Share";

                result = CopyDirectory(shareDirectory, directory, true);

                if (mData.HostName.ToLower().Contains("testuser") == true)
                {
                    PinvokeWindowsNetworking.disconnectRemote(@"\\" + mData.IPAddress + @"\testuser\Share");
                }
            }
            catch (Exception ex)
            {
                Log("Exception in CopyFilesLocally [" + mData.VMName + "]: " + ex);
                throw ex;
            }

            return result;
        }

        private string GetOutputDirectory(int testJobId, int testJobCommandId)
        {
            return Path.Combine(mOutputDirectory, "Output_" + testJobId + "_" + testJobCommandId);
        }

        private TestOutput ParseFilesLocally(ATCommandDoneRequest indication)
        {
            string directory = GetOutputDirectory(indication.TestJobID, indication.TestCommandID);
            TestOutput output = new TestOutput()
            {
                Results = new TestResults(),
                Version = string.Empty,
                DownloadLink = string.Empty
            };

            ParseOutputJson(directory, output);
            HandleDumpFiles(directory, indication, output);
            ParseWRLogFile(directory, output);

            Log("Results for TestJobCommand " + indication.TestCommandID + ": success = " + output.Results.Passed +
                "; failed: " + output.Results.Failed + "; number of dump files: " + output.NumberOfDumpFiles);

            return output;
        }

        // Method to parse the TEST log file to get the version and DownloadLink
        private void ParseWRLogFile(string directory, TestOutput output)
        {
            string testLogFile = Path.Combine(directory, Definitions.mWRLogFile);
            if (File.Exists(testLogFile) == true)
            {
                StreamReader reader = new StreamReader(testLogFile);
                while (reader.EndOfStream == false)
                {
                    string line = reader.ReadLine().ToLower();
                    if (line.Contains("installation successfully completed"))
                    {
                        try
                        {
                            int startIndex = line.IndexOf("(");
                            int stopIndex = line.IndexOf(")");
                            output.DownloadLink = line.Substring(startIndex + 1, stopIndex - startIndex - 3);

                            // Grab the release version
                            line = reader.ReadLine().ToLower();
                            startIndex = line.IndexOf("[");
                            stopIndex = line.IndexOf("]");
                            output.Version = line.Substring(startIndex + 2, stopIndex - startIndex - 2);
                        }
                        catch (Exception ex)
                        {
                            Log("Exception parsing LogFile.log: " + ex);
                        }

                        break;
                    }
                }
                reader.Dispose();

                Log("DownloadLink: " + output.DownloadLink + "; Version: " + output.Version);
            }
        }

        private void ParseOutputJson(string directory, TestOutput output)
        {
            string outputJson = Path.Combine(directory, Definitions.mOutputJson);
            if (File.Exists(outputJson) == true)
            {
                try
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TestOutput));
                    StreamReader reader = new StreamReader(outputJson);
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(reader.ReadToEnd()));
                    var result = (TestOutput)serializer.ReadObject(ms);
                    output.Results = result.Results;
                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    Log("Error parsing output.json file: " + ex.Message);
                    output.Results.Failed = 1;
                }
            }
        }

        private void HandleDumpFiles(string directory, ATCommandDoneRequest indication, TestOutput output)
        {
            var dumpFiles = Directory.GetFiles(directory, "TEST-s-*.dmp");
            if (dumpFiles != null)
            {
                output.NumberOfDumpFiles = dumpFiles.Length;
            }
            output.OutputPath = directory;

            if (output.NumberOfDumpFiles > 0)
            {
                try
                {
                    string dumpDirectory = Path.Combine(mDumpDirectory, indication.LicenseKey + "_" + mData.ConfigurationName);
                    dumpDirectory = Path.Combine(dumpDirectory, indication.TestJobID + "_" + indication.TestCommandID);
                    Directory.CreateDirectory(dumpDirectory);
                    CopyDirectory(directory, dumpDirectory, true);
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }

                // The following code creates a snapshot when there is a dump file for debugging purposes.
                // It is not being used right now.
                if (mCreateSnapshotWhenCrashed == true)
                {
                    // Create a snapshot
                    string snapshot = "DumpFile_" + indication.TestJobID + "_" + indication.TestCommandID;
                    mBusy = true;
                    Log("Creating snapshot for dump analysis with name " + snapshot);
                    CreateSnapshot(snapshot);
                    mBusy = false;
                }
            }
        }

        // TODO: move to common
        private bool CopyDirectory(string source, string target, bool copySubDirs)
        {
            Log("Copying " + source + " to " + target);

            try
            {
                DirectoryInfo info = new DirectoryInfo(source);
                if (info.Exists == false)
                {
                    Log("Remote directory '" + source + "' does not exist.");
                    return false;
                }

                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }

                FileInfo[] files = info.GetFiles();
                foreach (FileInfo fileInfo in files)
                {
                    string path = Path.Combine(target, fileInfo.Name);

                    try
                    {
                        Log("Copying " + fileInfo.Name + " to " + path);
                        fileInfo.CopyTo(path, true);
                    }
                    catch (Exception ex)
                    {
                        Log("Exception copying: " + ex);
                    }
                }

                if (copySubDirs == true)
                {
                    DirectoryInfo[] Directories = info.GetDirectories();
                    foreach (DirectoryInfo subdir in Directories)
                    {
                        string tempPath = Path.Combine(target, subdir.Name);
                        CopyDirectory(subdir.FullName, tempPath, copySubDirs);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Exception: " + ex);
                return false;
            }

            return true;
        }

        private void DeleteShareFiles(ATCommandDoneRequest indication)
        {
            string directory = GetOutputDirectory(indication.TestJobID, indication.TestCommandID);
            string[] files = Directory.GetFiles(directory, "*.db");
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }

        private void StartHeartbeatTimer()
        {
            lock (this)
            {
                if (mHeartbeatTimer == null)
                {
                    mHeartbeatsMissed = 0;
                    mHeartbeatTimer = new Timer(HeartbeatTimerTimeout, null, mHeartbeatTimerTimeoutMs, mHeartbeatTimerTimeoutMs);
                }
            }
        }

        private void StopHeartbeatTimer()
        {
            lock (this)
            {
                if (mHeartbeatTimer != null)
                {
                    mHeartbeatTimer.Dispose();
                    mHeartbeatTimer = null;
                    mHeartbeatsMissed = 0;
                }
            }
        }

        private void HeartbeatTimerTimeout(object state)
        {
            lock (this)
            {
                mHeartbeatsMissed++;
            }

            if (mHeartbeatsMissed > mMaxHeartBeatMissed)
            {
                mHeartbeatsMissed = 0;

                Log(mMaxHeartBeatMissed + " or more heartbeats missed for VM " + mData.VMName, true);
                TestJobData.Cancel(mData.VMInstanceID, "VM missed " + mMaxHeartBeatMissed + " heart beats.");

                try
                {
                    // Refresh data to see if the VM is Disabled
                    mData = VMInstanceData.SelectByIP(mData.IPAddress);
                    Log("Fetched updated information. VMState: " + mData.State);
                }
                catch (Exception ex)
                {
                    Log("ERROR: " + ex.ToString(), true);
                }

                if (mData.State == 1)
                {
                    mBusy = true;
                    //CreateSnapshot("HBFailure_" + DateTime.Now.ToString("yyyy-dd-MM-HH-mm-ss"));
                    mBusy = false;

                    CleanupVM();
                }
            }
            else if (mHeartbeatsMissed > 1)
            {
                Log("Heartbeats missed: " + mHeartbeatsMissed, true);
            }
        }

        private void StartLog()
        {
            mLogFile = new Logger(mData.VMName);
        }

        private void SendEmail(int testJobID, string userName)
        {
            try
            {
                var report = TestSuiteReport.CreateReport(testJobID);

                MailMessage message = new MailMessage();
                message.To.Add(userName + "@testapp.com");
                message.From = new MailAddress("testautomation@testapp.com");
                message.Subject = "Automated Test Results for JobID " + testJobID;
                message.IsBodyHtml = false;
                message.Body = report;

                SmtpClient client = new SmtpClient("smtprelay.services.testapp");
                client.UseDefaultCredentials = true;
                client.Send(message);
            }
            catch (Exception ex)
            {
                Log("Exception when sending email: " + ex);
            }
        }

        private bool Ping()
        {
            bool result = false;

            try
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = "ping.exe";
                process.StartInfo.Arguments = "-w 30000 " + mData.IPAddress;
                Log("Executing: " + process.StartInfo.FileName + " " + process.StartInfo.Arguments);

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Log("Result from Ping: " + output);
                if (output.ToLower().Contains("reply from") && output.ToLower().Contains("bytes=") == true)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Log("Exception: " + ex);
            }

            return result;
        }

        private bool ConnectRDP()
        {
            bool result = false;

            try
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "mstsc.exe";
                process.StartInfo.Arguments = "/v:" + mData.VMName;
                Log("Executing: " + process.StartInfo.FileName + " " + process.StartInfo.Arguments);
                process.Start();
                Log("ProcessID: " + process.Id);
                process.WaitForExit();

                ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));
                foreach (ManagementObject mo in mos.Get())
                {
                    mChildProcess = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));
                    Log("MSTSC Child Process Id: " + mChildProcess.Id);
                    break;
                }

                result = true;
            }
            catch (Exception ex)
            {
                Log("MSTSC exception: " + ex);
            }

            return result;
        }
    }
}

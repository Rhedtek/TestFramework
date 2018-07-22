using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using AutomationTestCommon;
using System.ServiceProcess;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct SYSTEMTIME
{
    public short wYear;
    public short wMonth;
    public short wDayOfWeek;
    public short wDay;
    public short wHour;
    public short wMinute;
    public short wSecond;
    public short wMilliseconds;
}


namespace AutomationTestClient
{
    class TestClient
    {
        private string mTestServer = string.Empty;
        private TcpClient mClient;
        private byte[] mHeaderData = new byte[8];
        private ATCommandResponse mCommandResponse;
        private const int mDelayBetweenRequestsMs = 20000;
        private bool mServiceState = true;  // remains true as long as no testcommand has been run by this instance
        private Timer mCommandTimeout = null;  // value comes from the ATCommandResponse
        private string mTargetDirectory = string.Empty;
        private string mShareDirectory = string.Empty;
        private string mProgramDataDirDirectory = string.Empty;
        private string mCommonFileDirectory = string.Empty;
        private string mTESTExecutable = string.Empty;
        private int mWaitForAPPSeconds = 30;
        private bool mCopiedCode = false;
        private Process mProcess;
        private ManualResetEventSlim mStdOutDone;
        private ManualResetEventSlim mStdErrDone;
        private StreamWriter mStdOut;
        private StreamWriter mStdErr;
        private delegate void safeMethod();
        private Timer mHeartbeatTimer = null;
        private const int mHeartbeatTimeoutMs = 60000;
        private StartClientResponse mStartClientResponse;

        // Keep track of command results
        private bool mCommandResult = false;
        private string mCommandResultString = string.Empty;

        public TestClient(string testServer)
        {
            mTestServer = testServer;
        }

        public void Start()
        {
            SendStartMessage();

            // The start response contains the operating system
            // ID needed to initialize some local variables
            InitializeVariables();

            SendLogMessage("MSG: TestClient Starting.");

            // Send the first message to indicate the client is up
            SendInitialMessage();

            try
            {
                // Start heartbeat timer
                StartHeartbeatTimer();

                // Sync with the network controller for the correct time
                SyncTime();

                while (true)
                {
                    Cleanup();
                    GetCommand();
                    if (mCommandResponse != null)
                    {
                        HandleCommand();
                    }

                    Thread.Sleep(mDelayBetweenRequestsMs);

                    // Sync with the network controller for the correct time
                    SyncTime();
                }
            }
            catch (Exception ex)
            {
                SendLogMessage("MSG: TestClient Exception: " + ex);
            }

            // If we ever get here, stop heartbeats; the server
            // will revert this VM to the Service snapshot
            mHeartbeatTimer.Dispose();
        }

        // Setup local variable based on operating system.
        private void InitializeVariables()
        {
            if (GetOperatingSystemType() == OperatingSystemType.Windows)
            {
                mTargetDirectory = "C:\\Packages";
                mShareDirectory = "C:\\Share";
                mProgramDataDirDirectory = "C:\\ProgramData\\ProgramDataDir";
                mCommonFileDirectory = "TestPackages\\CommonFiles";
                mTESTExecutable = "TEST.EXE";
            }
            else
            {
                mTargetDirectory = "/Users/testuser/Packages";
                mShareDirectory = "/Users/testuser/Share";
                mProgramDataDirDirectory = string.Empty;
                mCommonFileDirectory = "TestPackages/CommonFilesMac";
                mTESTExecutable = "TEST.dmg";
            }
        }

        private void StartHeartbeatTimer()
        {
            if (mHeartbeatTimer != null)
            {
                mHeartbeatTimer.Dispose();
                mHeartbeatTimer = null;
            }
            mHeartbeatTimer = new Timer(HeartbeatTimerTimeout, null, mHeartbeatTimeoutMs, mHeartbeatTimeoutMs);
        }

        private void HeartbeatTimerTimeout(object state)
        {
            SendHeartBeat();
        }

        private void SendHeartBeat()
        {
            try
            {
                Log("Sending heartbeat to " + mTestServer + ".");
                TcpClient client = new TcpClient(mTestServer, Definitions.mPortNumber);
                HeartbeatRequest request = new HeartbeatRequest()
                {
                    TestCommandID = (mCommandResponse == null ? -1 : mCommandResponse.TestCommandID),
                    RunCount = (mCommandResponse == null ? -1 : mCommandResponse.RunCount)
                };
                byte[] data = JsonHelper.ToByteStream<HeartbeatRequest>(request);
                Send(client, AutomationTestMessageID.HeartBeatRequest, data);

                client.Client.ReceiveTimeout = 10000;   // ms timeout
                int read = client.Client.Receive(mHeaderData, 8, SocketFlags.None);
                if (read > 0)
                {
                    // Data received, or connection closed
                    int dataLength = BitConverter.ToInt32(mHeaderData, 4);
                    data = new byte[dataLength];
                    client.Client.Receive(data, dataLength, SocketFlags.None);
                    var response = JsonHelper.ToObject<HeartbeatResponse>(data);
                    Log("Heartbeat response; TestRunning: " + response.TestRunning);

                    if (mProcess != null && response.TestRunning == false)
                    {
                        StopProcess("Process cancelled by database.");
                    }
                }

                client.Close();
            }
            catch { }
        }

        private void SendInitialMessage()
        {
            bool success = false;
            while (success == false)
            {
                try
                {
                    Log("Attempting to send the Initial message to the server.");

                    mClient = new TcpClient(mTestServer, Definitions.mPortNumber);
                    Send(mClient, AutomationTestMessageID.InitialMessageRequest, null);

                    mClient.Client.ReceiveTimeout = 10000;   // ms timeout
                    int read = mClient.Client.Receive(mHeaderData, 8, SocketFlags.None);
                    if (read > 0)
                    {
                        // Data received, or connection closed
                        int dataLength = BitConverter.ToInt32(mHeaderData, 4);
                        byte[] data = new byte[dataLength];
                        mClient.Client.Receive(data, dataLength, SocketFlags.None);
                        var response = JsonHelper.ToObject<InitialMessageResponse>(data);

                        Log("Response received for Initial message with timestamp " + response.Now);

                        // Do not set time if this is a MAC
                        if (GetOperatingSystemType() == OperatingSystemType.Windows)
                        {
                            SetCurrentTime(response.Now);
                        }

                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    Log("Initial message failed: " + ex);
                }
                CloseConnection();

                if (success == false)
                {
                    Thread.Sleep(mDelayBetweenRequestsMs);
                }
            }
        }

        private void SyncTime()
        {
            // Do not sync time if this is a MAC
            if (GetOperatingSystemType() == OperatingSystemType.Mac)
            {
                APPHelper.SetTimeMac();
                return;
            }

            int attempt = 0;

            ServiceController serviceController = new ServiceController("w32time");
            if (serviceController.Status != ServiceControllerStatus.Running)
            {
                serviceController.Start();
            }

            Log("w32time service is running");

            SendHeartBeat();

            while (attempt < 25)
            {
                try
                {
                    Process processTime = new Process();
                    processTime.StartInfo.FileName = "C:\\Windows\\System32\\w32tm.exe";
                    processTime.StartInfo.Arguments = "/resync /force";
                    processTime.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    processTime.StartInfo.UseShellExecute = false;
                    processTime.StartInfo.RedirectStandardOutput = true;
                    processTime.Start();
                    string output = processTime.StandardOutput.ReadToEnd();
                    processTime.WaitForExit();

                    Log("Synced time with server: " + output);
                    SendLogMessage("MSG: " + output);

                    if (output.ToLower().Contains("the command completed successfully.") == true)
                    {
                        // Due to the possible time change, the timer may be out of sync, so start a new one.
                        StartHeartbeatTimer();
                        return;
                    }

                    Log("Retrying time sync in 10 sec.");
                }
                catch (Exception ex)
                {
                    Log("Unable to sync time: " + ex);
                }

                Thread.Sleep(10000);
                attempt++;

                SendHeartBeat();
            }

            throw new Exception("MSG: Could not sync time.");
        }

        private static void SetCurrentTime(DateTime now)
        {
            Console.WriteLine("Setting local time to " + now);

            SYSTEMTIME time = new SYSTEMTIME();
            time.wYear = (short)now.Year;
            time.wMonth = (short)now.Month;
            time.wDay = (short)now.Day;
            time.wHour = (short)now.Hour;
            time.wMinute = (short)now.Minute;
            time.wSecond = (short)now.Second;
            SetSystemTime(ref time);
        }

        private void GetCommand()
        {
            SendCommandRequest();
            WaitForCommandResponse();
            CloseConnection();
        }

        private void SendCommandRequest()
        {
            bool successful = false;
            while (successful == false)
            {
                try
                {
                    mClient = new TcpClient(mTestServer, Definitions.mPortNumber);
                    Log("Sending CommandRequest; connected to " + mTestServer + ".");

                    ATCommandRequest request = new ATCommandRequest()
                    {
                        ServiceState = mServiceState
                    };

                    // Send the CommandRequest message with data
                    byte[] data = JsonHelper.ToByteStream<ATCommandRequest>(request);
                    Send(AutomationTestMessageID.GetCommandRequest, data);

                    // Bail out
                    successful = true;
                }
                catch
                {
                    Log("No response for CommandRequest.");
                    CloseConnection();
                    Thread.Sleep(mDelayBetweenRequestsMs);
                }
            }
        }

        private void WaitForCommandResponse()
        {
            try
            {
                mClient.Client.ReceiveTimeout = 10000;   // ms timeout
                int read = mClient.Client.Receive(mHeaderData, 8, SocketFlags.None);
                if (read > 0)
                {
                    // Data received, or connection closed
                    int dataLength = BitConverter.ToInt32(mHeaderData, 4);
                    byte[] data = new byte[dataLength];
                    mClient.Client.Receive(data, dataLength, SocketFlags.None);
                    mCommandResponse = JsonHelper.ToObject<ATCommandResponse>(data);

                    Log("Received command " + mCommandResponse.TestCommand);
                }
                else
                {
                    Log("Received zero bytes from the TestServer.");
                }
            }
            catch (Exception ex)
            {
                Log("No data received for CommandResponse: " + ex.Message);
            }
        }

        private void HandleCommand()
        {
            try
            {
                CreateOutputFiles();
                CleanupShare();

                mServiceState = false;
                Log("Handling command " + mCommandResponse.TestCommand);
                string[] result = mCommandResponse.TestCommand.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                string command = result[0].ToLower();
                if (command.StartsWith("installapp") == true)
                {
                    CopyCodeLocally();
                    HandleVersion();
                    installapp();
                }
                else if (command == "entrypoint")
                {
                    CopyCodeLocally();
                    RunEntryPoint(result[1]);
                }
                else
                {
                    Log("Invalid command received: " + command);
                }

                CloseLogFiles();
                CopyToShare();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
                mCommandResult = false;
                mCommandResultString += ex.ToString();
            }

            SendCommandDone();
            CloseConnection();
        }

        private bool CopyCodeLocally()
        {
            bool result = true;
            if (mCopiedCode == false)
            {
                string sourceDirectory = string.Empty;

                // Copy test package
                if (GetOperatingSystemType() == OperatingSystemType.Windows)
                {
                    sourceDirectory = "\\\\" + mTestServer + "\\" + mCommandResponse.TestPackageDirectory;
                }
                else
                {
                    sourceDirectory = "/Volumes/" + mCommandResponse.TestPackageDirectory.Replace('\\', '/');
                }

                Log("Copying data from " + sourceDirectory + " to " + mTargetDirectory);
                result = CopyDirectory(sourceDirectory, mTargetDirectory, true);

                // Copy common files
                if (GetOperatingSystemType() == OperatingSystemType.Windows)
                {
                    sourceDirectory = "\\\\" + mTestServer + "\\" + mCommonFileDirectory;
                }
                else
                {
                    sourceDirectory = "/Volumes/" + mCommonFileDirectory;
                }

                Log("Copying data from " + sourceDirectory + " to " + mTargetDirectory);
                result = CopyDirectory(sourceDirectory, mTargetDirectory, true);

                if (string.IsNullOrEmpty(mCommandResponse.TestSuiteDirectory) == false)
                {
                    if (GetOperatingSystemType() == OperatingSystemType.Windows)
                    {
                        sourceDirectory = "\\\\" + mTestServer + "\\" + mCommandResponse.TestSuiteDirectory;
                    }
                    else
                    {
                        sourceDirectory = "/Volumes/" + mCommandResponse.TestSuiteDirectory.Replace('\\', '/');
                    }

                    Log("Copying data from " + sourceDirectory + " to " + mTargetDirectory);
                    result = CopyDirectory(sourceDirectory, mTargetDirectory, true);
                }

                mCopiedCode = true;
            }

            return result;
        }

        private void HandleVersion()
        {
            string path = Path.Combine(mTargetDirectory, mTESTExecutable);

            if (mCommandResponse.Version.ToLower() == "latest")
            {
                string url = "http://anywhere.testappcloudav.com/zerol/" + mCommandResponse.DownloadLink.ToLower();

                Log("Requesting latest version of " + mCommandResponse.DownloadLink + " from " + url);
                try
                {
                    using (var client = new System.Net.WebClient())
                    {
                        client.DownloadFile(url, path);
                    }
                }
                catch (Exception ex)
                {
                    mCommandResultString += "APP Failed to download: " + ex.Message + "; ";
                    mCommandResult = false;
                }
                if (File.Exists(path) == false)
                {
                    mCommandResultString += "APP Failed to download;";
                    mCommandResult = false;
                }
            }
            else if (mCommandResponse.Version.ToLower() == "package")
            {
                string[] results = mCommandResponse.TestCommand.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (results.Length == 2)
                {
                    path = Path.Combine(mTargetDirectory, results[1]);
                }

                if (File.Exists(path) == false)
                {
                    mCommandResultString += "APP '" + path + "'was not part of the TestPackage" + "; ";
                    mCommandResult = false;
                }
            }
            else if (mCommandResponse.Version == "dragdrop")
            {
                Log("Using drag/drop version.");
            }
            else
            {
                Log("Unknown Version: " + mCommandResponse.Version);
                mCommandResultString += "APP version unknown: " + mCommandResponse.Version + "; ";
                mCommandResult = false;
            }
        }

        private void installapp()
        {
            string TESTExe = mTESTExecutable;

            string[] results = mCommandResponse.TestCommand.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (results.Length == 2)
            {
                TESTExe = Path.Combine(mTargetDirectory, results[1]);
            }

            Log("Install path: " + TESTExe);

            bool result = false;

            if (GetOperatingSystemType() == OperatingSystemType.Windows)
            {
                result = APPHelper.installapp(mTargetDirectory, TESTExe, mCommandResponse.LicenseKey);
            }
            else
            {
                result = APPHelper.installappMac(mTargetDirectory, TESTExe, mCommandResponse.LicenseKey);
            }

            if (result == false)
            {
                Log("APP failed to install");
                mCommandResultString += "APP Failed to install" + "; ";
                mCommandResult = false;
            }
            else
            {
                Log("Waiting for " + mWaitForAPPSeconds + " seconds to let APP start up");
                Thread.Sleep(mWaitForAPPSeconds * 1000);

                if (GetOperatingSystemType() == OperatingSystemType.Windows)
                {
                    result = APPHelper.CheckTESTIsRunning();
                    Log("TEST running: " + result);

                    if (result == false)
                    {
                        mCommandResultString += "APP is not running" + "; ";
                        mCommandResult = false;
                    }
                    else
                    {
                        result = Directory.Exists(mProgramDataDirDirectory);
                        Log("TEST ProgramDataDir exists: " + result);
                        if (result == false)
                        {
                            mCommandResultString += "APP Data directory was not created" + "; ";
                            mCommandResult = false;
                        }
                    }
                }
            }

            CreateOutputJson((result == true ? 1 : 0), (result == true ? 0 : 1));
        }

        // Commands that the Test Client handles and executes must also generate a JSON output file,
        // and this method is doing just that.
        private void CreateOutputJson(int passed, int failed)
        {
            TestOutput output = new TestOutput();

            TestResults result = new TestResults()
            {
                Passed = passed,
                Failed = failed,
                Warnings = 0,
                Skipped = 0,
                TestsExecuted = 1,
                StartTime = DateTime.Now.ToString(),
                EndTime = DateTime.Now.ToString()
            };
            output.Results = result;

            string path = Path.Combine(mTargetDirectory, Definitions.mOutputJson);
            StreamWriter writer = new StreamWriter(path);
            JsonHelper.ToStream<TestOutput>(writer.BaseStream, output);
            writer.Close();
        }

        private void RunEntryPoint(string entryPoint)
        {
            Log("Running " + entryPoint);

            Directory.SetCurrentDirectory(mTargetDirectory);

            // Start timer to make sure the process is not running forever
            mCommandTimeout = new Timer(ProcessTimeout, null, mCommandResponse.TimeoutMinutes * 60 * 1000, 0);
            mStdOutDone = new ManualResetEventSlim(false);
            mStdErrDone = new ManualResetEventSlim(false);

            mProcess = new Process();
            mProcess.StartInfo.WorkingDirectory = mTargetDirectory;

            string[] args = entryPoint.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // In case of a MAC, the EntryPoint may be defined as 'mono program.exe' or '/bin/bash script.sh'
            if (mStartClientResponse != null && mStartClientResponse.OSType == OperatingSystemType.Mac)
            {
                if (args.Length == 1)
                {
                    mProcess.StartInfo.FileName = Path.Combine(mTargetDirectory, args[0]);
                }
                else if (args.Length > 1)
                {
                    mProcess.StartInfo.FileName = args[0];
                    mProcess.StartInfo.Arguments = Path.Combine(mTargetDirectory, args[1]);
                }
            }
            else
            {
                mProcess.StartInfo.FileName = Path.Combine(mTargetDirectory, entryPoint);
            }

            Log("Starting exec= " + mProcess.StartInfo.FileName + "; Args=" + mProcess.StartInfo.Arguments);

            mProcess.StartInfo.UseShellExecute = false;
            mProcess.StartInfo.RedirectStandardOutput = true;
            mProcess.StartInfo.RedirectStandardError = true;

            Log("Setting LicenseKey environment variable to " + mCommandResponse.LicenseKey);
            mProcess.StartInfo.EnvironmentVariables["LicenseKey"] = mCommandResponse.LicenseKey;

            mProcess.OutputDataReceived += Process_OutputDataReceived;
            mProcess.ErrorDataReceived += Process_ErrorDataReceived;

            try
            {
                mProcess.Start();
                mProcess.BeginOutputReadLine();
                mProcess.BeginErrorReadLine();

                while (mProcess != null)
                {
                    mProcess.WaitForExit(1000);
                    if (mProcess.HasExited == true)
                    {
                        Log("Process has exited.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Exception during execution: " + ex);
                mCommandResultString += "Exception during execution: " + ex + "; ";
                mStdOutDone.Set();
                mStdErrDone.Set();
            }
            finally
            {
                if (mProcess != null)
                {
                    mProcess.Close();
                    mProcess = null;
                }

                if (mCommandTimeout != null)
                {
                    mCommandTimeout.Dispose();
                    mCommandTimeout = null;
                }

                // Wait a max of 10 seconds for the streams to end
                mStdOutDone.Wait(10000);
                mStdErrDone.Wait(10000);
            }

            Log("Done with executing the process.");
        }

        // Helper method that catches data is being streamed to StdErr
        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                mStdErrDone.Set();
            }
            else
            {
                Log("Error: " + e.Data);
                mStdErr.WriteLine(e.Data);
            }
        }

        // Helper method that catches data is being streamed to StdOut
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                mStdOutDone.Set();
            }
            else
            {
                Log("Output: " + e.Data);
                mStdOut.WriteLine(e.Data);
            }
        }

        private void ProcessTimeout(object state)
        {
            StopProcess("Process timed out.");
        }

        private void StopProcess(string message)
        {
            Log(message);

            mCommandResult = false;
            mCommandResultString = message;
			mCommandResponse.TimedOut = true;
			
            try
            {
                if (mCommandTimeout != null)
                {
                    mCommandTimeout.Dispose();
                    mCommandTimeout = null;
                }

                KillProcessAndChildren(mProcess.Id);
            }
            catch (Exception ex)
            {
                Log("Process termination exception: " + ex);
            }
        }

        private void KillProcessAndChildren(int pid)
        {
            Log("KillProcessAndChildren " + pid);

            ManagementObjectSearcher searcher = new ManagementObjectSearcher
              ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process process = Process.GetProcessById(pid);
                process.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

        private void SendCommandDone()
        {
            bool successful = false;
            while (successful == false)
            {
                try
                {
                    mClient = new TcpClient(mTestServer, Definitions.mPortNumber);
                    Log("Sending SendCommandDone; connected to " + mTestServer);

                    ATCommandDoneRequest result = new ATCommandDoneRequest()
                    {
                        TestSuiteID = mCommandResponse.TestSuiteID,
                        TestJobID = mCommandResponse.TestJobID,
                        TestCommandID = mCommandResponse.TestCommandID,
                        Result = mCommandResult,
                        ResultString = mCommandResultString,
                        Snapshot = mCommandResponse.Snapshot,
                        MaxExecutionOrder = mCommandResponse.MaxExecutionOrder,
                        ExecutionOrder = mCommandResponse.ExecutionOrder,
                        LicenseKey = mCommandResponse.LicenseKey,
                        UserName = mCommandResponse.UserName,
                        TimedOut = mCommandResponse.TimedOut
                    };
                    byte[] data = JsonHelper.ToByteStream<ATCommandDoneRequest>(result);

                    // Send the CommandDone request message with data
                    Send(AutomationTestMessageID.CommandDoneRequest, data);

                    // Wait for response
                    mClient.Client.ReceiveTimeout = 10000;   // ms timeout
                    mClient.Client.Receive(mHeaderData, 8, SocketFlags.None);

                    Log("CommandDoneResponse received.");

                    // Bail out
                    successful = true;
                }
                catch
                {
                    Log("Error when sending CommandDone message.");
                    CloseConnection();
                    Thread.Sleep(mDelayBetweenRequestsMs);
                }
            }
        }

        // This method cleans up the TestPackage directory after a test is done
        private void Cleanup()
        {
            mCommandResponse = null;
            mCommandResult = true;
            mCommandResultString = string.Empty;

            SafeExecute(() => { File.Delete(Path.Combine(mTargetDirectory, Definitions.mStdOut)); });
            SafeExecute(() => { File.Delete(Path.Combine(mTargetDirectory, Definitions.mErrLog)); });

            // Remove dump files
            if (Directory.Exists(mProgramDataDirDirectory))
            {
                string[] dumpFiles = Directory.GetFiles(mProgramDataDirDirectory, "*.dmp");
                foreach (var dumpFile in dumpFiles)
                {
                    Log("Deleting dump file " + dumpFile);
                    SafeExecute(() =>
                    {
                        File.Delete(dumpFile);
                    });
                }
            }
        }

        private void CloseConnection()
        {
            if (mClient != null)
            {
                mClient.Close();
                mClient = null;
            }
        }

        private void Send(AutomationTestMessageID messageID, byte[] data)
        {
            Send(mClient, messageID, data);
        }

        // Helper method to send a message to the server
        private void Send(TcpClient client, AutomationTestMessageID messageID, byte[] data)
        {
            byte[] buffer;

            if (data != null)
            {
                buffer = new byte[data.Length + 8];
                BitConverter.GetBytes((Int32)data.Length).CopyTo(buffer, 4);
                data.CopyTo(buffer, 8);
            }
            else
            {
                buffer = new byte[8];
                BitConverter.GetBytes((Int32)0).CopyTo(buffer, 4);
            }

            BitConverter.GetBytes((Int16)messageID).CopyTo(buffer, 0);
            BitConverter.GetBytes((Int16)78).CopyTo(buffer, 2);

            client.Client.Send(buffer);
            Log(string.Format("Sending messageId {0} dataLength {1}", messageID, buffer.Length));
        }

        private void Log(string msg)
        {
            msg = "[" + DateTime.Now + "] " + msg;
            Console.WriteLine(msg);
            if (mStdOut != null)
            {
                mStdOut.WriteLine(msg);
            }
        }

        private void CopyToShare()
        {
            Log("Copying data to Share");

            // Copy output files
            CopyToShare(Path.Combine(mTargetDirectory, AutomationTestCommon.Definitions.mOutputJson));
            CopyToShare(Path.Combine(mTargetDirectory, AutomationTestCommon.Definitions.mStdOut));
            CopyToShare(Path.Combine(mTargetDirectory, AutomationTestCommon.Definitions.mErrLog));

            if (Directory.Exists(mProgramDataDirDirectory))
            {
                CopyDirectory(mProgramDataDirDirectory, mShareDirectory, false);
            }
        }

        private void CleanupShare()
        {
            Log("Cleaning up share directory.");

            string[] files = Directory.GetFiles(mShareDirectory, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Log("Error in cleaning up share: " + ex);
                }
            }
        }

        private void CopyToShare(string file)
        {
            try
            {
                string filename = Path.GetFileName(file);
                string path = Path.Combine(mShareDirectory, filename);
                Log("Copying " + file + " to " + path);
                File.Copy(file, path, true);
            }
            catch (Exception ex)
            {
                mCommandResultString += "Exception when copying files to Share: " + ex + "; ";
            }
        }

        private bool CopyDirectory(string source, string target, bool copySubDirs)
        {
            try
            {
                DirectoryInfo sourceInfo = new DirectoryInfo(source);

                if (sourceInfo.Exists == false)
                {
                    Log("Remote directory '" + source + "' does not exist.");
                    mCommandResultString += "Remote directory '" + source + "' does not exist" + "; ";
                    return false;
                }

                // If the destination directory doesn't exist, create it.
                DirectoryInfo targetInfo = new DirectoryInfo(target);
                if (targetInfo.Exists == false)
                {
                    Directory.CreateDirectory(target);
                }

                FileInfo[] files = sourceInfo.GetFiles();
                foreach (FileInfo fileInfo in files)
                {
                    // Just to make sure we are not copying over any output.json files
                    if (fileInfo.FullName.Contains(Definitions.mOutputJson)) continue;

                    string path = Path.Combine(target, fileInfo.Name);
                    //if (File.Exists(path) == false)
                    {
                        try
                        {
                            fileInfo.CopyTo(path, true);
                            Log("Copying " + fileInfo.Name + " to " + path);
                        }
                        catch (Exception ex)
                        {
                            Log("Error during copying of " + fileInfo.FullName + ": " + ex.Message);
                        }
                    }
                }

                if (copySubDirs == true)
                {
                    DirectoryInfo[] Directories = sourceInfo.GetDirectories();
                    foreach (DirectoryInfo subdir in Directories)
                    {
                        string tempPath = Path.Combine(target, subdir.Name);
                        CopyDirectory(subdir.FullName, tempPath, copySubDirs);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Exception during copying: " + ex);
                mCommandResultString += "Exception during copying: " + ex.Message + "; ";
                return false;
            }

            return true;
        }

        private void CreateOutputFiles()
        {
            mStdOut = new StreamWriter(Path.Combine(mTargetDirectory, Definitions.mStdOut));
            mStdErr = new StreamWriter(Path.Combine(mTargetDirectory, Definitions.mErrLog));
        }

        private void CloseLogFiles()
        {
            mStdErr.Close();
            mStdErr = null;

            mStdOut.Close();
            mStdOut = null;
        }

        private void SafeExecute(safeMethod f)
        {
            try
            {
                f();
            }
            catch { }
        }

        private void SendLogMessage(string message)
        {
            try
            {
                TcpClient client = new TcpClient(mTestServer, Definitions.mPortNumber);
                LogMessageIndication request = new LogMessageIndication()
                {
                    Message = message
                };
                byte[] data = JsonHelper.ToByteStream<LogMessageIndication>(request);
                Send(client, AutomationTestMessageID.LogIndication, data);
                client.Close();
            }
            catch
            {
                // Nothing
            }
        }

        private OperatingSystemType GetOperatingSystemType()
        {
            if (mStartClientResponse == null)
                return OperatingSystemType.Windows;
            return mStartClientResponse.OSType;
        }

        private void SendStartMessage()
        {
            // Send Start message, wait for returned state = enabled
            bool enabled = false;
            while (enabled == false)
            {
                try
                {
                    Log("Sending Start message.");

                    // Send message
                    TcpClient client = new TcpClient(mTestServer, Definitions.mPortNumber);
                    Send(client, AutomationTestMessageID.StartMessageRequest, null);

                    byte[] buffer = new byte[8];

                    // The timeout is important to prevent a socket from getting stuck
                    client.Client.ReceiveTimeout = 10000;   // ms timeout
                    int read = client.Client.Receive(buffer, 8, SocketFlags.None);
                    if (read > 0)
                    {
                        // Wait for response
                        int dataLength = BitConverter.ToInt32(buffer, 4);
                        buffer = new byte[dataLength];
                        client.Client.Receive(buffer, dataLength, SocketFlags.None);
                        mStartClientResponse = JsonHelper.ToObject<StartClientResponse>(buffer);
                        enabled = mStartClientResponse.Enabled;

                        Log("Start response: enable=" + enabled + ", OS=" + mStartClientResponse.OSType);
                    }

                    client.Close();
                }
                catch (Exception ex)
                {
                    Log("Exception: " + ex);
                }

                if (enabled == false)
                {
                    Thread.Sleep(30000);
                }
            }

            Console.WriteLine("VM enabled for OS " + mStartClientResponse.OSType);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);
    }
}

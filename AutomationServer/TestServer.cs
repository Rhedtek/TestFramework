using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using AutomationTestCommon;
using AutomationTestServer.DataObjects;

namespace AutomationTestServer
{
    public class TestServer
    {
        private Logger mLogFile;
        private Timer mCleanupTimer;
        private const int mCleanupTimeoutMs = 1 * 60 * 60 * 1000;  // 1 hour of 60 minutes of 60 seconds of 1000 ms
        private Timer mScheduleTimer;
        private const int mScheduleTimeoutMs = 15 * 60 * 1000;  // 1 hour of 15 minutes of 60 seconds of 1000 ms
        private static string disk_l = File.Exists("E:\\") ? "E:\\" : "C:\\";
        private string mOutputDirectory = disk_l + "Output";
        private const int mDeleteDirectoryThresholdDays = 7;
        private PeriodicReports mPeriodicReports;
        private TestServerState mTestServerState = TestServerState.Unknown;
        private VMRequestReceiver mVMRequestReceiver;
        private StatusRequestReceiver mStatusRequestReceiver;

        #region Public methods
        public TestServerState TestServerState
        {
            get
            {
                return mTestServerState;
            }
        }

        public void Start()
        {
            mTestServerState = TestServerState.Initializing;

            // Start the receiver/listener sckets for VM communication, and GUI requests
            mVMRequestReceiver = new VMRequestReceiver(this);
            mStatusRequestReceiver = new StatusRequestReceiver(this);

            StartLog();                 // Logging for this process
            DatabaseServerStart();      // Download data from the database

            mVMRequestReceiver.Start();
            mStatusRequestReceiver.Start();

            StartTimers();
            StartPeriodicReports();

            mTestServerState = TestServerState.Running;
        }

        public void Stop()
        {
            Log("Stop called.");
            mTestServerState = TestServerState.Stopping;

            // Close the listener sockets, stop accepting new connections
            mVMRequestReceiver.Stop();
            mStatusRequestReceiver.Stop();

            // Kill mstsc.exe processes since these are child processes of this process
            KillChildProcesses();

            Log("Done.", true);
            mTestServerState = TestServerState.Stopped;
        }
        #endregion

        #region Internal methods
        internal void Log(string txt, bool stdOut = false)
        {
            var logTxt = "[" + DateTime.Now + ":" + Thread.CurrentThread.ManagedThreadId + "] " + txt;

            if (stdOut == true)
            {
                Console.WriteLine(logTxt);
            }

            lock (this)
            {
                mLogFile.Log(logTxt);
            }
        }
        #endregion

        private void StartLog()
        {
            mLogFile = new Logger("Service");
        }

        private void StartTimers()
        {
            mCleanupTimer = new Timer(CleanupTimerTimeout, null, mCleanupTimeoutMs, mCleanupTimeoutMs);
            mScheduleTimer = new Timer(ScheduleTimerTimeout, null, mScheduleTimeoutMs, mScheduleTimeoutMs);
        }

        private void ScheduleTimerTimeout(object state)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("[Active].[CheckTestSuiteSchedule]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Failure during scheduling action: " + ex);
            }
        }

        // This method (attempts to) clean up directories older than a certain amount of time.
        private void CleanupTimerTimeout(object state)
        {
            Log("Cleaning up directories.");

            string[] results = Directory.GetDirectories(mOutputDirectory);
            foreach (string result in results)
            {
                var dateTime = Directory.GetCreationTime(result);
                if (DateTime.Now.Subtract(dateTime).Days > mDeleteDirectoryThresholdDays)
                {
                    try
                    {
                        Directory.Delete(result, true);
                    }
                    catch (Exception ex)
                    {
                        Log("Failed to delete directory " + result + ": " + ex.Message);
                    }
                }
            }
        }

        // Inform the database that the server has started.
        private void DatabaseServerStart()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[TestServer_Start]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
            }
        }

        // This process is responsible for sending period reports
        private void StartPeriodicReports()
        {
            mPeriodicReports = new PeriodicReports();
            mPeriodicReports.Start(this);
        }

        private void KillChildProcesses()
        {
            var procs = System.Diagnostics.Process.GetProcessesByName("mstsc.exe");
            foreach (var proc in procs)
            {
                Log("Killing process with ID " + proc.Id);

                try
                {
                    proc.Kill();
                }
                catch (Exception ex)
                {
                    Log("Error while trying to kill a process with ID " + proc.Id + ": " + ex);
                }
            }
        }
    }
}

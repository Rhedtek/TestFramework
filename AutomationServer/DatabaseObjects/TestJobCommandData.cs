using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using AutomationTestCommon;

namespace AutomationTestServer
{
    internal class TestJobCommandData
    {
        internal int TestSuiteID { get; private set; }
        internal int TestJobID { get; private set; }
        internal int TestCommandID { get; private set; }
        internal string TestCommandString { get; private set; }
        internal int ExecutionOrder { get; set; }
        internal int TimeoutMinutes { get; private set; }
        internal string TestPackageDirectory { get; private set; }
        internal string Version { get; private set; }
        internal string LicenseKey { get; private set; }
        internal string DownloadLink { get; private set; }
        internal string TestSuiteDirectory { get; set; }
        internal int MaxExecutionOrder { get; set; }
        internal string UserName { get; private set; }
        internal int RunCount { get; private set; }

        internal static TestJobCommandData GetNextCommand(int vmInstanceID)
        {
            TestJobCommandData testCommand = null;
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[GetNextCommand]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pVMInstanceID", vmInstanceID);

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        testCommand = FromData(reader);
                    }
                    reader.Close();
                }
            }

            return testCommand;
        }

        internal int MarkAsDone(int vmInstanceID, TestResults testResults, bool dumpFiles, string message, string version, string DownloadLink)
        {
            return MarkAsDone(vmInstanceID, this.TestCommandID, testResults, dumpFiles, message, version, DownloadLink);
        }

        internal static int MarkAsDone(int vmInstanceID, int testCommandID, TestResults testResults, bool dumpFiles, string message, string version, string DownloadLink)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[TestJobCommand_Done]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestJobCommandID", testCommandID);
                    command.Parameters.AddWithValue("@pVMInstanceID", vmInstanceID);
                    command.Parameters.AddWithValue("@pPassedCount", testResults.Passed);
                    command.Parameters.AddWithValue("@pWarningCount", testResults.Warnings);
                    command.Parameters.AddWithValue("@pFailedCount", testResults.Failed);
                    command.Parameters.AddWithValue("@pResultString", message);
                    command.Parameters.AddWithValue("@pDumpFilesGenerated", dumpFiles);
                    command.Parameters.AddWithValue("@pVersion", version);
                    command.Parameters.AddWithValue("@pDownloadLink", DownloadLink);
                    command.Parameters.AddWithValue("@pSkippedCount", testResults.Skipped);

                    SqlParameter param = new SqlParameter("@oTestJobState", SqlDbType.Int);
                    param.Direction = ParameterDirection.Output;
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();

                    return (int)param.Value;
                }
            }
        }

        internal static bool IsRunning(int testCommandID, int runCount)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;
            bool result = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[TestJobCommand_IsRunning]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestJobCommandID", testCommandID);
                    command.Parameters.AddWithValue("@pRunCount", runCount);
                    int returnCode = (int)command.ExecuteScalar();
                    result = (returnCode > 0);
                }
            }

            return result;
        }

        internal void AbortCommand()
        {

        }

        private static TestJobCommandData FromData(IDataReader reader)
        {
            TestJobCommandData testCommand = new TestJobCommandData();
            testCommand.TestJobID = reader.GetInt32(0);
            testCommand.TestCommandID = reader.GetInt32(1);
            testCommand.TestCommandString = reader.GetString(2);
            testCommand.ExecutionOrder = reader.GetInt32(3);
            testCommand.TimeoutMinutes = reader.GetInt32(4);
            testCommand.TestPackageDirectory = reader.GetString(5);
            testCommand.Version = reader.GetString(6);
            testCommand.LicenseKey = reader.GetString(7);
            testCommand.DownloadLink = reader.GetString(8);
            testCommand.TestSuiteDirectory = (reader.IsDBNull(9) ? string.Empty : reader.GetString(9));
            testCommand.MaxExecutionOrder = reader.GetInt32(10);
            testCommand.TestSuiteID = reader.GetInt32(11);
            testCommand.UserName = reader.GetString(12);
            testCommand.RunCount = reader.GetInt32(13);
            return testCommand;
        }
    }
}

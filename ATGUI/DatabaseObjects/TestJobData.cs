using System;
using System.Collections.Generic;
using System.Configuration;

using System.Data;
using System.Data.SqlClient;

namespace ATGUI.DatabaseObjects
{
    public class TestJobData
    {
        public int TestSuiteID { get; private set; }
        public int TestJobID { get; private set; }
        public int ConfigurationID { get; private set; }
        public string LicenseKey { get; private set; }
        public string DownloadLink { get; private set; }
        public DateTime CreateDate { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public string TestSuiteName { get; private set; }
        public int Priority { get; private set; }
        public int TestPackageID { get; private set; }
        public int TestPackageVersionID { get; private set; }
        public string VMName { get; set; }
        public string ConfigurationString { get; private set; }
        public string PackageName { get; private set; }
        public DateTime? StartTime { get; private set; }
        public string Version { get; private set; }
        public string State { get; private set; }
        public int TestJobState { get; private set; }

        public static int Create(int testSuiteID, int testPackageID, int configurationID, string licenseKey, string DownloadLink, int priority, string Version, string testSuiteDirectory)
        {
            int testJobID = -1;
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestJob_Create]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuiteID", testSuiteID);
                    command.Parameters.AddWithValue("@pTestPackageID", testPackageID);
                    command.Parameters.AddWithValue("@pConfigurationID", configurationID);
                    command.Parameters.AddWithValue("@pLicenseKey", licenseKey);
                    command.Parameters.AddWithValue("@pDownloadLink", DownloadLink);
                    command.Parameters.AddWithValue("@pPriority", priority);
                    command.Parameters.AddWithValue("@pVersion", Version);
                    command.Parameters.AddWithValue("@pTestSuiteDirectory", testSuiteDirectory);

                    SqlParameter param = new SqlParameter("@oTestJobID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Output;
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                    testJobID = (int)param.Value;
                }
            }

            return testJobID;
        }

        public static IEnumerable<TestJobData> Select(int? testSuiteID, int? testJobID, int operatingSystemID)
        {
            List<TestJobData> result = new List<TestJobData>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestJob_Select]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuiteID", testSuiteID);
                    command.Parameters.AddWithValue("@pTestJobID", testJobID);
                    command.Parameters.AddWithValue("@pOperatingSystemID", operatingSystemID);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var testInstance = FromData(reader);
                        result.Add(testInstance);
                    }
                    reader.Close();
                }
            }

            return result;
        }

        public void Reset(string message)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[TestJob_Reset]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestJobID", TestJobID);
                    command.Parameters.AddWithValue("@pMessage", message);
                    command.ExecuteNonQuery();
                }
            }
        }

        internal void Cancel(string message)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[TestJob_Cancel]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestJobID", TestJobID);
                    command.Parameters.AddWithValue("@pVMInstanceID", null);
                    command.Parameters.AddWithValue("@pMessage", message);
                    command.ExecuteNonQuery();
                }
            }
        }

        internal void Pause()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[TestJob_Pause]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestJobID", TestJobID);
                    command.ExecuteNonQuery();
                }
            }
        }

        internal void Start()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[TestJob_Start]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestJobID", TestJobID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public string Duration
        {
            get
            {
                if (StartDate != null)
                {
                    if (EndDate != null)
                    {
                        var duration = (DateTime)EndDate - (DateTime)StartDate;
                        return duration.Hours.ToString("D2") + "h" + duration.Minutes.ToString("D2") + "m" + duration.Seconds.ToString("D2") + "s";
                    }
                    else
                    {
                        var duration = DateTime.Now - (DateTime)StartDate;
                        return duration.Hours.ToString("D2") + "h" + duration.Minutes.ToString("D2") + "m" + duration.Seconds.ToString("D2") + "s";
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private static TestJobData FromData(IDataReader reader)
        {
            TestJobData testJob = new TestJobData();
            testJob.TestJobID = reader.GetInt32(0);
            testJob.TestSuiteID = reader.GetInt32(1);
            testJob.ConfigurationID = reader.GetInt32(2);
            testJob.LicenseKey = reader.GetString(3);
            testJob.DownloadLink = reader.GetString(4);
            testJob.CreateDate = reader.GetDateTime(5);
            testJob.StartDate = (reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6));
            testJob.EndDate = (reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7));
            testJob.TestSuiteName = reader.GetString(8);
            testJob.Priority = reader.GetInt32(9);
            testJob.TestPackageID = reader.GetInt32(10);
            testJob.TestPackageVersionID = reader.GetInt32(11);
            testJob.VMName = (reader.IsDBNull(12) ? string.Empty : reader.GetString(12));
            testJob.ConfigurationString = reader.GetString(13);
            testJob.PackageName = reader.GetString(14);
            testJob.StartTime = (reader.IsDBNull(15) ? (DateTime?)null : reader.GetDateTime(15));
            testJob.Version = reader.GetString(16);
            testJob.State = reader.GetString(17);
            testJob.TestJobState = reader.GetInt32(18);
            return testJob;
        }
    }
}

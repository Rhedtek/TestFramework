using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ATGUI.DatabaseObjects
{
    public class TestSuiteData
    {
        public int TestSuiteID { get; private set; }
        public string TestSuiteName { get; private set; }
        public string TestSuiteDescription { get; private set; }
        public string TestPackage { get; private set; }
        public DateTime CreateDate { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public int TestPackageID { get; private set; }
        public int TestPackageVersionID { get; private set; }
        public string AddedByUser { get; private set; }
        public int? PassedCount { get; private set; }
        public int? WarningCount { get; private set; }
        public int? FailedCount { get; private set; }
        public int State { get; private set; }
        public int? Limit { get; private set; }
        public DateTime? StartTime { get; private set; }

        public string StateString
        {
            get
            {
                switch (State)
                {
                    case 1:
                        return "Scheduled";
                    case 2:
                        return "Running";
                    case 3:
                        return "Done";
                    case 4:
                        return "Paused";
                    case 5:
                        return "Cancelled";
                    case 6:
                        return "Deleted";
                    default:
                        return "Unknown";
                }
            }
        }

        public static IEnumerable<TestSuiteData> Select(int? testSuiteID, int operatingSystemID)
        {
            List<TestSuiteData> result = new List<TestSuiteData>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestSuite_Select]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuiteID", testSuiteID);
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

        public static int Create(string name, string description, int packageID, int priority, int? maxRunning, int operatingSystemID)
        {
            int testSuiteID = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestSuite_Create]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuiteName", name);
                    command.Parameters.AddWithValue("@pTestSuiteDescription", description);
                    command.Parameters.AddWithValue("@pTestPackageID", packageID);
                    command.Parameters.AddWithValue("@pUser", Environment.UserName);
                    command.Parameters.AddWithValue("@pTestPriority", priority);
                    command.Parameters.AddWithValue("@pMaxRunning", maxRunning);
                    command.Parameters.AddWithValue("@pOperatingSystemID", operatingSystemID);

                    SqlParameter param = new SqlParameter("@oTestSuiteID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Output;
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                    testSuiteID = (int)param.Value;
                }
            }

            return testSuiteID;
        }

        public void Cancel()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[TestSuite_Cancel]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuiteID ", TestSuiteID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[TestSuite_Delete]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuiteID ", TestSuiteID);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static TestSuiteData FromData(IDataReader reader)
        {
            TestSuiteData instance = new TestSuiteData();
            instance.TestSuiteID = reader.GetInt32(0);
            instance.TestSuiteName = reader.GetString(1);
            instance.TestSuiteDescription = reader.GetString(2);
            instance.TestPackage = reader.GetString(3);
            instance.CreateDate = reader.GetDateTime(4);
            instance.StartDate = (reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5));
            instance.EndDate = (reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6));
            instance.TestPackageID = reader.GetInt32(7);
            instance.TestPackageVersionID = reader.GetInt32(8);
            instance.AddedByUser = reader.GetString(9);
            instance.PassedCount = (reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10));
            instance.WarningCount = (reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11));
            instance.FailedCount = (reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12));
            instance.State = reader.GetInt32(13);
            instance.Limit = (reader.IsDBNull(14) ? (int?)null : reader.GetInt32(14));
            instance.StartTime = (reader.IsDBNull(15) ? (DateTime?)null : reader.GetDateTime(15));
            return instance;
        }
    }
}

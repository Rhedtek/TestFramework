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
    public class TestSuitePlanData
    {
        public int TestSuitePlanID { get; private set; }
        public string TestSuitePlanName { get; private set; }
        public string TestSuitePlanDescription { get; private set; }
        public string TestPackage { get; private set; }
        public DateTime CreateDate { get; private set; }
        public int TestPackageID { get; private set; }
        public string AddedByUser { get; private set; }
        public string State { get; private set; }

        public static IEnumerable<TestSuitePlanData> Select()
        {
            List<TestSuitePlanData> result = new List<TestSuitePlanData>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestSuitePlan_Select]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
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

                using (SqlCommand command = new SqlCommand("[Active].[TestSuitePlan_Create]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuitePlanName", name);
                    command.Parameters.AddWithValue("@pTestSuitePlanDescription", description);
                    command.Parameters.AddWithValue("@pTestPackageID", packageID);
                    command.Parameters.AddWithValue("@pTestPriority", priority);
                    command.Parameters.AddWithValue("@pUser", Environment.UserName);
                    command.Parameters.AddWithValue("@pMaxRunning", maxRunning);
                    command.Parameters.AddWithValue("@pOperatingSystemID", operatingSystemID);

                    SqlParameter param = new SqlParameter("@oTestSuitePlanID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Output;
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                    testSuiteID = (int)param.Value;
                }
            }

            return testSuiteID;
        }

        public void Enable()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestSuitePlan_Enable]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuitePlanID", TestSuitePlanID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Disable()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestSuitePlan_Disable]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuitePlanID", TestSuitePlanID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Cancel()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestSuitePlan_Cancel]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuitePlanID", TestSuitePlanID);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static TestSuitePlanData FromData(IDataReader reader)
        {
            TestSuitePlanData instance = new TestSuitePlanData();
            instance.TestSuitePlanID = reader.GetInt32(0);
            instance.TestSuitePlanName = reader.GetString(1);
            instance.TestSuitePlanDescription = reader.GetString(2);
            instance.TestPackage = reader.GetString(3);
            instance.CreateDate = reader.GetDateTime(4);
            instance.AddedByUser = reader.GetString(5);
            instance.State = reader.GetString(6);
            return instance;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ATGUI.DatabaseObjects
{
    public class TestPackageData
    {
        public int TestPackageID { get; private set; }
        public string TestPackageName { get; private set; }
        public string TestPackageDescription { get; private set; }
        public int CurrentVersion { get; private set; }
        public string LicenseKey { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public int ServerGroupID { get; private set; }
        public string AutomationLabIDString { get; private set; }
        public int AutomationLabID { get; private set; }

        public static IEnumerable<TestPackageData> Select(int operatingSystemID, int labID, int? testPackageID)
        {
            List<TestPackageData> result = new List<TestPackageData>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Definition].[TestPackage_Select]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestPackageID", testPackageID);
                    command.Parameters.AddWithValue("@pOperatingSystemID", operatingSystemID);
                    command.Parameters.AddWithValue("@pLabID", labID);

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

        public void Disable()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Definition].[TestPackage_Disable]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestPackageID", TestPackageID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static int Create(string name, string description, string path, string licenseKey, int serverGroupID, int operatingSystemID, int labID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Definition].[TestPackage_Create]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestPackageName", name);
                    command.Parameters.AddWithValue("@pTestPackageDescription", description);
                    command.Parameters.AddWithValue("@pTestPackagePath", path);
                    command.Parameters.AddWithValue("@pLicenseKey", licenseKey);
                    command.Parameters.AddWithValue("@pServerGroupID", serverGroupID);
                    command.Parameters.AddWithValue("@pOperatingSystemID", operatingSystemID);
                    //Requires new column in TestPackage table: LabID
                    command.Parameters.AddWithValue("@pLabID", labID);
                    SqlParameter param1 = new SqlParameter("@oTestPackageID", SqlDbType.Int);
                    param1.Direction = ParameterDirection.Output;
                    SqlParameter param2 = new SqlParameter("@oTestPackageVersionID", SqlDbType.Int);
                    param2.Direction = ParameterDirection.Output;
                    command.Parameters.Add(param1);
                    command.Parameters.Add(param2);
                    command.ExecuteNonQuery();

                    return (int)param1.Value;
                }
            }
        }

        private static TestPackageData FromData(IDataReader reader)
        {
            TestPackageData testPackage = new TestPackageData();
            testPackage.TestPackageID = reader.GetInt32(0);
            testPackage.TestPackageName = reader.GetString(1);
            testPackage.TestPackageDescription = reader.GetString(2);
            testPackage.CurrentVersion = reader.GetInt32(5);
            testPackage.LicenseKey = (reader.IsDBNull(6) ? string.Empty : reader.GetString(6));
            testPackage.LastUpdate = reader.GetDateTime(7);
            testPackage.ServerGroupID = (reader.IsDBNull(8) ? 1 : reader.GetInt32(8));
            testPackage.AutomationLabIDString = (reader.IsDBNull(9) ? "Development" : reader.GetString(9));
            testPackage.AutomationLabID = reader.GetInt32(10);
            return testPackage;
        }
    }
}

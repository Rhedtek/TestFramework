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
    public class TestJobPlanData
    {
        public int TestSuitePlanID { get; private set; }
        public int TestJobPlanID { get; private set; }
        public int ConfigurationID { get; private set; }
        public string LicenseKey { get; private set; }
        public string DownloadLink { get; private set; }
        public DateTime CreateDate { get; private set; }
        public string TestSuitePlanName { get; private set; }
        public int Priority { get; private set; }
        public int TestPackageID { get; private set; }
        public string ConfigurationString { get; private set; }
        public string PackageName { get; private set; }
        public DateTime? StartTime { get; private set; }

        public static int Create(int testSuiteID, int testPackageID, int configurationID, string licenseKey, string DownloadLink, int priority, string Version, string testSuiteDirectory)
        {
            int testJobID = -1;
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestJobPlan_Create]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuitePlanID", testSuiteID);
                    command.Parameters.AddWithValue("@pTestPackageID", testPackageID);
                    command.Parameters.AddWithValue("@pConfigurationID", configurationID);
                    command.Parameters.AddWithValue("@pLicenseKey", licenseKey);
                    command.Parameters.AddWithValue("@pDownloadLink", DownloadLink);
                    command.Parameters.AddWithValue("@pPriority", priority);
                    command.Parameters.AddWithValue("@pVersion", Version);

                    SqlParameter param = new SqlParameter("@oTestJobPlanID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Output;
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                    testJobID = (int)param.Value;
                }
            }

            return testJobID;
        }
    }
}

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace AutomationTestServer.DataObjects
{
    public class TestSuiteReport
    {
        public int TestSuiteID { get; set; }
        public int PassedCount { get; private set; }
        public int WarningCount { get; private set; }
        public int FailedCount { get; private set; }
        public string EntryPoint { get; private set; }
        public int RunCount { get; private set; }

        public static string CreateReport(int testJobID)
        {
            string report = string.Empty;
            var results = Select(testJobID);

            report = "Your TestJob with ID " + testJobID + " has finished. Results are shown below:\n\n";
            foreach (var entry in results)
            {
                report += "Command=" + entry.EntryPoint + ": result=" + entry.Result + ". Number of runs: " + entry.RunCount + "; passed: " + entry.PassedCount +
                    "; warning: " + entry.WarningCount + "; failed: " + entry.FailedCount + "\n";
            }

            return report;
        }

        private string FailedRatio
        {
            get
            {
                int total = PassedCount + WarningCount + FailedCount;
                if (total == 0)
                    return "0.00";
                double ratio = (double)FailedCount / (double)total * 100.0;
                return ratio.ToString("F2");
            }
        }

        private string Result
        {
            get
            {
                if (FailedCount > 0)
                {
                    return "Failed";
                }
                if (WarningCount > 0)
                {
                    return "Warning";
                }
                return "Passed";
            }
        }
        private static IEnumerable<TestSuiteReport> Select(int testJobID)
        {
            List<TestSuiteReport> result = new List<TestSuiteReport>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestJobReport_Select]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestJobID", testJobID);
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

        private static TestSuiteReport FromData(IDataReader reader)
        {
            TestSuiteReport instance = new TestSuiteReport();
            instance.TestSuiteID = reader.GetInt32(0);
            instance.EntryPoint = reader.GetString(1);
            instance.PassedCount = (reader.IsDBNull(2) ? 0 : reader.GetInt32(2));
            instance.WarningCount = (reader.IsDBNull(3) ? 0 : reader.GetInt32(3));
            instance.FailedCount = (reader.IsDBNull(4) ? 0 : reader.GetInt32(4));
            instance.RunCount = (reader.IsDBNull(5) ? 0 : reader.GetInt32(5));
            return instance;
        }
    }
}

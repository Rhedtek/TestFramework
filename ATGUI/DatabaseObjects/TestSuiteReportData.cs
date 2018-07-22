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
    public class TestSuiteReportData
    {
        public int TestSuiteID { get; set; }
        public int PassedCount { get; private set; }
        public int WarningCount { get; private set; }
        public int FailedCount { get; private set; }
        public string EntryPoint { get; private set; }
        public int RunCount { get; private set; }

        public string FailedRatio
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

        public string Result
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
        public static IEnumerable<TestSuiteReportData> Select(int? testSuiteID)
        {
            List<TestSuiteReportData> result = new List<TestSuiteReportData>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestSuiteReport_Select]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuiteID", testSuiteID);
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

        private static TestSuiteReportData FromData(IDataReader reader)
        {
            TestSuiteReportData instance = new TestSuiteReportData();
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

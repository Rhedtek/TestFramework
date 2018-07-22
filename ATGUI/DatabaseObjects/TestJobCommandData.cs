using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media;

namespace ATGUI.DatabaseObjects
{
    public class TestJobCommandData
    {
        public int TestJobCommandID { get; private set; }
        public int TestSuiteID { get; private set; }
        public int TestJobID { get; private set; }
        public int ConfigurationID { get; private set; }
        public string EntryPoint { get; private set; }
        public string LicenseKey { get; set; }
        public DateTime CreateDate { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public int? PassedCount { get; private set; }
        public int? WarningCount { get; private set; }
        public int? FailedCount { get; private set; }
        public int? SkippedCount { get; private set; }
        public string ResultString { get; private set; }
        public string VMName { get; private set; }
        internal string PackagePath { get; private set; }
        public bool? DumpFilesGenerated { get; private set; }
        public string Configuration { get; private set; }
        public string Version { get; private set; }
        public string DownloadLink { get; private set; }

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

        public SolidColorBrush SelectedRowColor
        {
            get
            {
                if (DumpFilesGenerated ?? false)
                    return Brushes.MediumPurple;
                if (StartDate == null)
                    return Brushes.LightGray;
                if (EndDate == null)
                    return Brushes.LightYellow;
                if (FailedCount > 0)
                    return Brushes.Red;
                if (WarningCount > 0)
                    return Brushes.Orange;
                if (PassedCount >= 0)
                    return Brushes.LawnGreen;
                if (SkippedCount > 0)
                    return Brushes.Yellow;

                return Brushes.LightSlateGray;
            }
        }

        public static void Create(int testJobID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestJobCommand_Create]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestJobID", testJobID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static IEnumerable<TestJobCommandData> Select(int? testSuiteID, int? testJobID, int operatingSystemID)
        {
            List<TestJobCommandData> result = new List<TestJobCommandData>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestJobCommand_Select]", connection))
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

        private static TestJobCommandData FromData(IDataReader reader)
        {
            TestJobCommandData testInstance = new TestJobCommandData();
            testInstance.TestJobCommandID = reader.GetInt32(0);
            testInstance.TestSuiteID = reader.GetInt32(1);
            testInstance.TestJobID = reader.GetInt32(2);
            testInstance.ConfigurationID = reader.GetInt32(3);
            testInstance.EntryPoint = reader.GetString(4);
            testInstance.LicenseKey = reader.GetString(5);
            testInstance.CreateDate = reader.GetDateTime(6);
            testInstance.StartDate = (reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7));
            testInstance.EndDate = (reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8));
            testInstance.PassedCount = (reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9));
            testInstance.WarningCount = (reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10));
            testInstance.FailedCount = (reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11));
            testInstance.ResultString = (reader.IsDBNull(12) ? string.Empty : reader.GetString(12));
            testInstance.VMName = (reader.IsDBNull(13) ? string.Empty : reader.GetString(13));
            testInstance.PackagePath = (reader.IsDBNull(14) ? string.Empty : reader.GetString(14));
            testInstance.DumpFilesGenerated = (reader.IsDBNull(15) ? (bool?)null : reader.GetBoolean(15));
            testInstance.Configuration = (reader.IsDBNull(16) ? string.Empty : reader.GetString(16));
            testInstance.Version = (reader.IsDBNull(17) ? string.Empty : reader.GetString(17));
            testInstance.DownloadLink = (reader.IsDBNull(18) ? string.Empty : reader.GetString(18));
            testInstance.SkippedCount = (reader.IsDBNull(19) ? (int?)null : reader.GetInt32(19));
            return testInstance;
        }
    }
}

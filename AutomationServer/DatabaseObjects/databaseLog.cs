using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AutomationTestServer.DatabaseObjects
{
    public class DatabaseLog
    {
        public static void Insert(string logEntry, string logDetails, int logLevel)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[Log_Insert]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pLogEntry", logEntry);
                    command.Parameters.AddWithValue("@pLogDetails", logDetails);
                    command.Parameters.AddWithValue("@pLogLevel", logLevel);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

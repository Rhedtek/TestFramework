using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace AutomationTestServer
{
    internal class TestJobData
    {
        internal static void Cancel(int vmInstanceID, string message)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("[Active].[TestJob_Cancel]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@pTestJobID", null);
                        command.Parameters.AddWithValue("@pVMInstanceID", vmInstanceID);
                        command.Parameters.AddWithValue("@pMessage", message);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal static void SetStopOnFailure(int testJobID, bool stopOnError)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[TestJob_SetStopOnError]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestJobID", testJobID);
                    command.Parameters.AddWithValue("@pStopOnError", stopOnError);
                    command.ExecuteNonQuery();
                }
            }
        }

        internal static void SetTimeout(int testJobID, int timeout)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Active].[TestJob_SetTimeout]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestJobID", testJobID);
                    command.Parameters.AddWithValue("@pTimeout", timeout);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

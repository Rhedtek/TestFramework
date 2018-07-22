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
    internal class TestPackageCommand
    {
        internal static void Create(int testPackageID, string testCommand, int executionOrder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Definition].[TestPackageCommand_Create]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestPackageID", testPackageID);
                    command.Parameters.AddWithValue("@pTestCommand", testCommand);
                    command.Parameters.AddWithValue("@pExecutionOrder", executionOrder);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

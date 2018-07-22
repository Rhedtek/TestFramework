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
    internal class TestPackageVersionData
    {
        internal int TestPackageID { get; private set; }
        internal int TestPackageVersionID { get; private set; }
        internal string TestPackagePath { get; private set; }

        internal static TestPackageVersionData Select(int testPackageID)
        {
            TestPackageVersionData result = null;
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Definition].[TestPackageVersion_Select]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestPackageID", testPackageID);

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = FromData(reader);
                    }
                    reader.Close();
                }
            }

            return result;
        }

        private static TestPackageVersionData FromData(IDataReader reader)
        {
            TestPackageVersionData instance = new TestPackageVersionData();
            instance.TestPackageID = reader.GetInt32(0);
            instance.TestPackageVersionID = reader.GetInt32(1);
            instance.TestPackagePath = reader.GetString(2);
            return instance;
        }
    }
}

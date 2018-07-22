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
    public class TestSuiteScheduleData
    {
        public static int Create(int testSuitePlanID)
        {
            int testPlanScheduleID = -1;

            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestPlanSchedule_Create]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuitePlanID", testSuitePlanID);
                    SqlParameter param = new SqlParameter("@oTestSuiteScheduleID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Output;
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                    testPlanScheduleID = (int)param.Value;
                }
            }

            return testPlanScheduleID;
        }
    }
}

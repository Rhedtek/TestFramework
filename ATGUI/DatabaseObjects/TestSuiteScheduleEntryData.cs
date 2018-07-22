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
    public class TestSuiteScheduleEntryData
    {
        public static void Create(int testPlanScheduleID, int weekDayID, int startingHour, int startingMinute)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[TestPlanScheduleEntry_Create]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pTestSuiteScheduleID", testPlanScheduleID);
                    command.Parameters.AddWithValue("@pWeekDayID", weekDayID);
                    command.Parameters.AddWithValue("@pStartingHour", startingHour);
                    command.Parameters.AddWithValue("@pStartingMinute", startingMinute);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace AutomationTestServer.DatabaseObjects
{
    public class PeriodicReportData
    {
        public int PeriodicReportID { get; private set; }
        public string ScriptPath { get; private set; }
        public string Recipients { get; private set; }
        public string EmailHeader { get; private set; }
        public string EmailBody { get; private set; }
        public DayOfWeek ScheduleDay { get; private set; }
        public int ScheduleHour { get; private set; }
        public int ScheduleMinute { get; private set; }
        public int PeriodicReportStatus { get; private set; }

        public static List<PeriodicReportData> Select()
        {
            List<PeriodicReportData> result = new List<PeriodicReportData>();

            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Definition].[PeriodicReport_Select]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var instance = FromData(reader);
                        result.Add(instance);
                    }
                    reader.Close();
                }
            }

            return result;
        }

        private static PeriodicReportData FromData(IDataReader reader)
        {
            PeriodicReportData instance = new PeriodicReportData();
            instance.PeriodicReportID = reader.GetInt32(0);
            instance.ScriptPath = reader.GetString(1);
            instance.Recipients = reader.GetString(2);
            instance.EmailHeader = reader.GetString(3);
            instance.EmailBody = reader.GetString(4);
            instance.ScheduleDay = (DayOfWeek)reader.GetInt32(5);
            instance.ScheduleHour = reader.GetInt32(6);
            instance.ScheduleMinute = reader.GetInt32(7);
            instance.PeriodicReportStatus = reader.GetInt32(8);
            return instance;
        }
    }
}

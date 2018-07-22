using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ATGUI.DatabaseObjects
{
    public class VMStatisticData
    {
        public string Label { get; private set; }
        public string TestJobID { get; private set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public string TestPackageName { get; private set; }
        public int? MaxRunning { get; private set; }
        public string User { get; private set; }
        public int? TestPriority { get; private set; }
        public int TestSuiteID { get; private set; }

        public static List<VMStatisticData> Select(int vmInstanceID)
        {
            List<VMStatisticData> result = new List<VMStatisticData>();
            bool resultsReturned = false;

            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[Active].[Queue_Select]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pVMInstanceID", vmInstanceID);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var testInstance = FromData(reader);
                        result.Add(testInstance);
                        resultsReturned = true;
                    }
                    reader.NextResult();
                    for (int index = result.Count(); index < 5; ++index)
                    {
                        result.Add(new VMStatisticData() { });
                    }
                    result.Reverse();
                    result.Last().Label = "Previous:";
                    resultsReturned = false;

                    // Next batch
                    while (reader.Read())
                    {
                        var testInstance = FromData(reader);
                        result.Add(testInstance);
                        resultsReturned = true;
                    }
                    if (resultsReturned == false)
                    {
                        result.Add(new VMStatisticData() { });
                    }
                    reader.NextResult();
                    if (result.LastOrDefault() != null)
                        result.Last().Label = "Current:";
                    resultsReturned = false;

                    while (reader.Read())
                    {
                        var testInstance = FromData(reader);
                        result.Add(testInstance);
                        if (resultsReturned == false && result.LastOrDefault() != null)
                        {
                            result.Last().Label = "Next:";
                        }
                        resultsReturned = true;
                    }
                    if (resultsReturned == false)
                    {
                        result.Add(new VMStatisticData() { Label = "Next:" });
                    }
                    for (int index = result.Count(); index < 11; ++index)
                    {
                        result.Add(new VMStatisticData() { });
                    }

                    reader.Close();
                }
            }

            return result;
        }

        private static VMStatisticData FromData(IDataReader reader)
        {
            VMStatisticData data = new VMStatisticData();
            data.TestJobID = reader.GetInt32(0).ToString();
            data.CreateDate = reader.GetDateTime(1);
            data.StartDate = (reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2));
            data.EndDate = (reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3));
            data.TestPackageName = reader.GetString(4);
            data.MaxRunning = (reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5));
            data.User = reader.GetString(6);
            data.TestPriority = reader.GetInt32(7);
            data.TestSuiteID = reader.GetInt32(8);
            return data;
        }
    }
}

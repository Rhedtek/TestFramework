using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using AutomationTestCommon;

namespace AutomationTestServer
{
    internal class VMInstanceData
    {
        internal int VMInstanceID { get; private set; }
        internal string VMName { get; private set; }
        internal string HostName { get; private set; }
        internal string IPAddress { get; private set; }
        internal int VMConfigurationID { get; private set; }
        internal int State { get; set; }
        internal bool AlwaysOn { get; private set; }
        internal string ConfigurationName { get; private set; }
        internal string Location { get; private set; }
        internal OperatingSystemType OperatingSystemID { get; private set; }

        public static VMInstanceData SelectByIP(string ipAddress)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;
            VMInstanceData result = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Definition].[VMInstance_SelectByIP]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pIPAddress", ipAddress);

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

        public static List<VMInstanceData> Select()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;
            List<VMInstanceData> result = new List<VMInstanceData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Definition].[VMInstance_Select]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var data = FromData(reader);
                        result.Add(data);
                    }
                    reader.Close();
                }
            }

            return result;
        }

        public void Start()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Definition].[VMInstance_Start]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pVMInstanceID", VMInstanceID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public string Inactive()
        {
            string vmCommand = string.Empty;
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Definition].[VMInstance_Start]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pVMInstanceID", VMInstanceID);
                    SqlParameter param = new SqlParameter("@oCommand", SqlDbType.VarChar);
                    param.Direction = ParameterDirection.Output;
                    command.Parameters.Add(param);
                    command.ExecuteNonQuery();
                    vmCommand = (string)param.Value;
                }
            }

            return vmCommand;
        }

        private static VMInstanceData FromData(IDataReader reader)
        {
            VMInstanceData instance = new VMInstanceData();
            instance.VMInstanceID = reader.GetInt32(0);
            instance.VMName = reader.GetString(1);
            instance.HostName = reader.GetString(2);
            instance.IPAddress = reader.GetString(3);
            instance.VMConfigurationID = reader.GetInt32(4);
            instance.State = reader.GetInt32(5);
            // 6: CreateDate
            instance.AlwaysOn = reader.GetBoolean(7);
            instance.ConfigurationName = reader.GetString(8);
            // 9: TestSuiteID
            // 10: testJobID
            instance.Location = reader.GetString(14);
            instance.OperatingSystemID = (reader.GetInt32(17) == 1 ? OperatingSystemType.Windows : OperatingSystemType.Mac);
            return instance;
        }
    }
}

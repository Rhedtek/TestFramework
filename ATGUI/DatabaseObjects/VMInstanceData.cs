using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media;

namespace ATGUI.DatabaseObjects
{
    public class VMInstanceData
    {
        public int VMInstanceID { get; private set; }
        public string VMName { get; private set; }
        public string HostName { get; private set; }
        public string IPAddress { get; private set; }
        public int VMConfigurationID { get; private set; }
        public int State { get; set; }
        public bool AlwaysOn { get; private set; }
        public string ConfigurationName { get; private set; }
        internal int? TestSuiteID { get; private set; }
        internal int? TestJobID { get; private set; }
        public int? TestJobCommandID { get; private set; }
        public string TestPackage { get; private set; }
        public string LabID { get; private set; }
        public DateTime? LastHeartbeat { get; private set; }

        private DateTime LastRefresh { get; set; }

        public string VMState
        {
            get
            {
                if (State == 0)
                    return "Disabled";
                if (State == 1)
                    return "Enabled";
                return "Unknown";
            }
        }

        public Visibility EnableVisibility
        {
            get
            {
                return (State == 1 ? Visibility.Hidden : Visibility.Visible);
            }
        }

        public Visibility DisableVisibility
        {
            get
            {
                return (State == 0 ? Visibility.Hidden : Visibility.Visible);
            }
        }

        public SolidColorBrush SelectedRowColor
        {
            get
            {
                if (LastHeartbeat == null)
                    return Brushes.Red;

                double minuteDiff = (LastRefresh - (DateTime)LastHeartbeat).TotalMinutes;
                if (minuteDiff > 10.0)
                {
                    if (State == 0)
                        return Brushes.DarkGray;
                    return Brushes.Red;
                }

                if (minuteDiff > 5.0)
                    return Brushes.Yellow;
                return Brushes.White;
            }
        }

        public static List<VMInstanceData> Select(int operatingSystemID, int? serverGroupID)
        {
            List<VMInstanceData> result = new List<VMInstanceData>();

            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Definition].[VMInstance_Select]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pOperatingSystemID", operatingSystemID);
                    command.Parameters.AddWithValue("@pServerGroupID", serverGroupID);

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

        public void Disable()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Definition].[VMInstance_Disable]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pVMInstanceID", VMInstanceID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Enable()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Definition].[VMInstance_Enable]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pVMInstanceID", VMInstanceID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void MoveToProduction()
        {
            MoveToLab(1);
        }

        public void MoveToDevelopment()
        {
            MoveToLab(2);
        }

        public string Running
        {
            get
            {
                if (TestJobID == null)
                {
                    if (TestJobCommandID == null)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return "Waiting for TestJobCommand '" + TestJobCommandID + "' to finish";
                    }
                }
                return "TestSuiteID: " + TestSuiteID + ", TestJobID: " + TestJobID;
            }
        }

        private void MoveToLab(int labID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[Definition].[VMInstance_SetAutomationLab]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pVMInstanceID", VMInstanceID);
                    command.Parameters.AddWithValue("@pAutomationLabID", labID);
                    command.ExecuteNonQuery();
                }
            }
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
            instance.AlwaysOn = reader.GetBoolean(7);
            instance.ConfigurationName = reader.GetString(8);
            instance.TestSuiteID = (reader.IsDBNull(9) ? null : (int?)reader.GetInt32(9));
            instance.TestJobID = (reader.IsDBNull(10) ? null : (int?)reader.GetInt32(10));
            instance.TestJobCommandID = (reader.IsDBNull(11) ? null : (int?)reader.GetInt32(11));
            instance.TestPackage = (reader.IsDBNull(12) ? string.Empty : reader.GetString(12));
            instance.LabID = (reader.IsDBNull(15) ? string.Empty : reader.GetString(15));
            instance.LastHeartbeat = (reader.IsDBNull(16) ? null : (DateTime?)reader.GetDateTime(16));
            instance.LastRefresh = DateTime.Now;
            return instance;
        }
    }
}

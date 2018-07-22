using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ComponentModel;
using System.Net.Sockets;
using ATGUI.DatabaseObjects;
using ATGUI.DataObjects;
using AutomationTestCommon;

namespace ATGUI
{
    /// <summary>
    /// Interaction logic for Packages.xaml
    /// </summary>
    public partial class Packages : Page
    {
        private enum TestServerRequestState
        {
            Idle,
            StopRequested,
            Stopping,
            Stopped
        }

        private ObservableCollection<TestPackageData> mPackages = new ObservableCollection<TestPackageData>();
        private List<TestPackageData> mOriginalData = new List<TestPackageData>();
        private string currentSortedColumn = string.Empty;
        private ListSortDirection currentDir = ListSortDirection.Descending;
        private MainWindow mMainWindow;
        private int mOperatingSystemID;
        private int mLabID;
        private TestServerRequestState mTestServerRequestState = TestServerRequestState.Idle;
        private System.Threading.Timer mStatusTimer;
        private const int mStatusTimeout = 30000;

        public Packages()
        {
            InitializeComponent();

            PackagesList.ItemsSource = mPackages;
        }

        public void Initialize(MainWindow mainWindow)
        {
            mMainWindow = mainWindow;

            // Setting this will trigger a changeselected event, which will update the other tabs
            OperatingSystemCB.SelectedIndex = ATGUI.Properties.Settings.Default.OperatingSystemID - 1;
            mOperatingSystemID = ATGUI.Properties.Settings.Default.OperatingSystemID;
            LabCB.SelectedIndex = ATGUI.Properties.Settings.Default.LabID - 1;
            mLabID = ATGUI.Properties.Settings.Default.LabID;

            mStatusTimer = new System.Threading.Timer(StatusTimeout, null, 1000, mStatusTimeout);
        }

        private void PackagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void Refresh()
        {
            try
            {
                mOriginalData.Clear();
                var result = TestPackageData.Select(mOperatingSystemID, mLabID, null);
                mOriginalData.AddRange(result);
                SortData();
                UpdateList();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Exception: " + ex);
            }
        }

        private void UpdateList()
        {
            mPackages.Clear();
            mPackages.AddRange(mOriginalData);
        }

        private void SortData()
        {
            if (currentSortedColumn == PackageNameColumnHeader.Name)
            {
                SortList(info => info.TestPackageName);
            }
            else if (currentSortedColumn == PackageDescriptionColumnHeader.Name)
            {
                SortList(info => info.TestPackageDescription);
            }
            else if (currentSortedColumn == PackageVersionColumnHeader.Name)
            {
                SortList(info => info.CurrentVersion);
            }
            else if (currentSortedColumn == LastUpdateColumnHeader.Name)
            {
                SortList(info => info.LastUpdate);
            }
            else if (currentSortedColumn == LastUpdateColumnHeader.Name)
            {
                SortList(info => info.LastUpdate);
            }
            //else if (currentSortedColumn == LabIDColumnHeader.Name)
            //{
            //    SortList(info => info.AutomationLabIDString);
            //}
        }

        private void SortList<T>(Func<TestPackageData, T> func)
        {
            if (currentDir == ListSortDirection.Ascending)
            {
                mOriginalData = mOriginalData.OrderBy(func).ToList();
            }
            else
            {
                mOriginalData = mOriginalData.OrderByDescending(func).ToList();
            }
        }

        private void SortClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            String field = column.Name as String;

            if (currentSortedColumn != field)
            {
                currentDir = ListSortDirection.Descending;
                currentSortedColumn = field;
            }
            else
            {
                if (currentDir == ListSortDirection.Ascending)
                    currentDir = ListSortDirection.Descending;
                else currentDir = ListSortDirection.Ascending;
            }

            SortData();
            UpdateList();
        }

        private void CreateTestSuiteContextMenu_Click(object sender, RoutedEventArgs e)
        {
            HandleTestSuiteCreate(false);
        }

        private void HandleTestSuiteCreate(bool scheduled)
        {
            if (PackagesList.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Please select a package to start a test suite from.");
                return;
            }

            CreateTestSuite suite = new CreateTestSuite();
            TestPackageData item = PackagesList.SelectedItem as TestPackageData;
            if (item != null)
            {
                var result = suite.Initialize(item, scheduled, mOperatingSystemID);
                if (result == true)
                {
                    suite.Show();
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Select item is null.");
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.Description = "Select the directory containing the packages to upload.";
            dialog.ShowNewFolderButton = false;

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.Cancel) return;

            if (dialog.SelectedPath.StartsWith("C:\\TestPackages") == true)
            {
                System.Windows.MessageBox.Show("TestPackages cannot be stored under C:\\TestPackages.");
                return;
            }

            string path = string.Empty;
            string[] jsonFiles = null;

            try
            {
                jsonFiles = Directory.GetFiles(dialog.SelectedPath, "*.json");
            }
            catch
            {
                System.Windows.MessageBox.Show("No test package found.");
                return;
            }

            if (jsonFiles == null || jsonFiles.Length == 0)
            {
                System.Windows.MessageBox.Show("No json files found.");
                return;
            }

            if (jsonFiles.Length > 1)
            {
                PackageJsonSelect jsonSelect = new PackageJsonSelect();
                jsonSelect.Initialize(jsonFiles);
                jsonSelect.ShowDialog();
                path = jsonSelect.SelectedFile();
            }
            else
            {
                path = jsonFiles[0];
            }

            if (string.IsNullOrEmpty(path))
                return;

            if (File.Exists(path) == false)
            {
                System.Windows.MessageBox.Show("The file '" + path + "' was not found.");
                return;
            }

            TestPackageJson package = null;
            StreamReader reader = null;

            // Parse the json file
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TestPackageJson));
                reader = new StreamReader(path);
                package = (TestPackageJson)serializer.ReadObject(reader.BaseStream);
                reader.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failure when parsing '" + path + "': " + ex);
                return;
            }

            if (reader != null)
                reader.Close();

            if (string.IsNullOrEmpty(package.ServerGroup) == true)
            {
                System.Windows.MessageBox.Show("Please specify a ServerGroup for this test package to use.");
                return;
            }

            int serverGroupID = GetServerGroupID(package.ServerGroup);
            if (serverGroupID == -1)
            {
                System.Windows.MessageBox.Show("Unknown server group: " + package.ServerGroup);
                return;
            }

            if (string.IsNullOrEmpty(package.Lab) == true)
            {
                System.Windows.MessageBox.Show("Please specify the Production or Development lab for this test package");
                return;
            }

            int labID = GetLabID(package.Lab);
            if (labID == -1)
            {
                System.Windows.MessageBox.Show("Unknown lab environment: " + package.Lab);
                return;
            }

            try
            {
                // Locate all entry points; verify snapshots
                Dictionary<string, int> snapshots = new Dictionary<string, int>();
                foreach (var command in package.Commands)
                {
                    string[] results = command.Split(new char[] { '=', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (command.ToLower().StartsWith("entrypoint="))
                    {
                        if (results.Length < 2)
                        {
                            System.Windows.MessageBox.Show("Invalid format for EntryPoint: " + command);
                            return;
                        }
                        path = System.IO.Path.Combine(dialog.SelectedPath, results[results.Length - 1]);
                        if (File.Exists(path) == false)
                        {
                            System.Windows.MessageBox.Show("Did not find entrypoint '" + results[1] + "'.");
                            return;
                        }
                    }
                    else if (command.ToLower().StartsWith("createsnapshot"))
                    {
                        if (results.Length < 2)
                        {
                            System.Windows.MessageBox.Show("Invalid format for CreateSnapshot: " + command);
                            return;
                        }
                        snapshots.Add(results[1], 0);
                    }
                    else if (command.ToLower().StartsWith("rollback"))
                    {
                        if (results.Length > 1)
                        {
                            if (snapshots.ContainsKey(results[1]) == false)
                            {
                                System.Windows.MessageBox.Show("Rolling back to non-existing snapshot: " + command);
                                return;
                            }
                            else
                            {
                                snapshots[results[1]]++;
                            }
                        }
                    }
                    else if (command.ToLower().StartsWith("installapp") == true)
                    {
                        if (results.Length > 1)
                        {
                            bool filePresent = FileInPackage(dialog.SelectedPath, results[1]);
                            if (filePresent == false)
                            {
                                System.Windows.MessageBox.Show("The command '" + command + "' refers to a file that is not part of the package.");
                                return;
                            }
                        }
                    }
                    else if (command.ToLower().StartsWith("stoponerror") == true)
                    {
                        bool stopOnError = false;
                        if (bool.TryParse(results[1], out stopOnError) == false)
                        {
                            System.Windows.MessageBox.Show("Invalid value for StopOnError: " + command);
                            return;
                        }
                    }
                    else if (command.ToLower().StartsWith("reboot") == true)
                    {
                        // OK
                    }
                    else if (command.ToLower().StartsWith("timeout") == true)
                    {
                        if (results.Length > 1)
                        {
                            int timeout = 0;
                            if (int.TryParse(results[1], out timeout) == false)
                            {
                                System.Windows.MessageBox.Show("Invalid value for Timeout: " + results[1]);
                                return;
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("No value specified for Timeout.");
                            return;
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Invalid command: " + command);
                        return;
                    }
                }

                foreach (KeyValuePair<string, int> kvp in snapshots)
                {
                    if (kvp.Value == 0)
                    {
                        System.Windows.MessageBox.Show("Snapshot with name '" + kvp.Key + "' is created but never used.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error when parsing package json: " + ex);
                return;
            }

            try
            {
                // Upload to database
                path = "C:\\TestPackages\\" + package.Name;  // dialog.SelectedPath.Replace("C:\\", string.Empty);
                int testPackageID = TestPackageData.Create(package.Name, package.Description, path.Replace("C:\\", string.Empty), package.LicenseKey, serverGroupID, mOperatingSystemID, labID);
                TestPackageVersionData testPackageVersion = TestPackageVersionData.Select(testPackageID);

                int executionOrder = 0;
                // Handle all commands
                foreach (string command in package.Commands)
                {
                    TestPackageCommand.Create(testPackageID, command, executionOrder);
                    executionOrder++;
                }

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                path = path + "\\" + testPackageVersion.TestPackageVersionID;
                System.Windows.MessageBox.Show("Renaming " + dialog.SelectedPath + " to " + path);
                Directory.Move(dialog.SelectedPath, path);

                DirectorySecurity sec = Directory.GetAccessControl(path);
                // Using this instead of the "Everyone" string means we work on non-English systems.
                SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                Directory.SetAccessControl(path, sec);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Exception: " + ex);
            }

            Refresh();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void DisablePackageCM_Click(object sender, RoutedEventArgs e)
        {
            if (PackagesList.SelectedItem != null)
            {
                var package = PackagesList.SelectedItem as TestPackageData;
                var result = System.Windows.MessageBox.Show("Are you sure you want to disable package '" + package.TestPackageName + "'?", "Disable Package", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    package.Disable();
                    Refresh();
                }
            }
        }

        private void CreateScheduledTestSuiteCM_Click(object sender, RoutedEventArgs e)
        {
            HandleTestSuiteCreate(true);
        }

        private bool FileInPackage(string directory, string file)
        {
            string path = System.IO.Path.Combine(directory, file);
            return File.Exists(path);
        }

        private int GetServerGroupID(string serverGroup)
        {
            string tag = serverGroup.ToLower();
            switch (tag)
            {
                case "consumer":
                    return 1;
                case "sme unmanaged":
                    return 2;
                case "sme managed":
                    return 3;
                case "long running":
                    return 4;
                case "test":
                    return 5;
                default:
                    return -1;
            }
        }

        private int GetLabID(string Lab)
        {
            string tag = Lab.ToLower();
            switch (tag)
            {
                case "production":
                    return 1;
                case "development":
                    return 2;
                case "suvp":
                    return 4;
                default:
                    return -1;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OperatingSystemCB.SelectedItem != null)
            {
                var item = OperatingSystemCB.SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    mOperatingSystemID = int.Parse(item.Tag as string);

                    // Inform the parent object
                    mMainWindow.SetOperatingSystem(mOperatingSystemID);

                    ATGUI.Properties.Settings.Default.OperatingSystemID = mOperatingSystemID;
                    ATGUI.Properties.Settings.Default.Save();
                }
            }
        }
        private void ComboBox_LabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LabCB.SelectedItem != null)
            {
                var item = LabCB.SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    mLabID = int.Parse(item.Tag as string);

                    // Inform the parent object
                    mMainWindow.SetLab(mLabID);
                    ATGUI.Properties.Settings.Default.LabID = mLabID;
                    ATGUI.Properties.Settings.Default.Save();
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show("Are you sure you want to stop the Test Server process?" +
                " This will completely shutdown the automation framework.", "Stop Test Server Process", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                if (mTestServerRequestState == TestServerRequestState.Idle)
                {
                    mTestServerRequestState = TestServerRequestState.StopRequested;

                    try
                    {
                        // Send stop request
                        byte[] headerData = new byte[8];
                        TcpClient client = new TcpClient(Environment.MachineName, Definitions.mTestServerControlPortNumber);
                        Send(client, (int)TestServerMessageID.TestServerStopReq, null);

                        // Wait for response
                        client.Client.ReceiveTimeout = 1000;
                        int read = client.Client.Receive(headerData, 8, SocketFlags.None);
                        if (read > 0)
                        {
                            UpdateStatus("Stopped");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Exception during server stop request: " + ex);
                    }
                }
            }
        }

        private void StatusTimeout(object state)
        {
            byte[] headerData = new byte[8];
            byte[] data;

            UpdateStatus("Checking...");

            try
            {
                // Send message to the test server
                TcpClient client = new TcpClient(Environment.MachineName, Definitions.mTestServerControlPortNumber);
                Send(client, (int)TestServerMessageID.TestServerStatusReq, null);

                // Wait for response
                client.Client.ReceiveTimeout = 1000;
                int read = client.Client.Receive(headerData, 8, SocketFlags.None);
                if (read > 0)
                {
                    // Data received, or connection closed
                    int dataLength = BitConverter.ToInt32(headerData, 4);
                    data = new byte[dataLength];
                    client.Client.Receive(data, dataLength, SocketFlags.None);
                    var response = JsonHelper.ToObject<TestServerStatusRsp>(data);
                    UpdateStatus(response.Status);
                }
                else
                {
                    UpdateStatus("Unknown");
                }
            }
            catch
            {
                UpdateStatus("Connection refused");
            }
        }

        private void UpdateStatus(string status)
        {
            ServerStatusLbl.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate ()
                    {
                        ServerStatusLbl.Content = status;
                    }));
        }

        // Duplicate function; todo: move to common place
        private void Send(TcpClient client, int messageID, byte[] data)
        {
            byte[] buffer;

            if (data != null)
            {
                buffer = new byte[data.Length + 8];
                BitConverter.GetBytes((Int32)data.Length).CopyTo(buffer, 4);
                data.CopyTo(buffer, 8);
            }
            else
            {
                buffer = new byte[8];
                BitConverter.GetBytes((Int32)0).CopyTo(buffer, 4);
            }

            BitConverter.GetBytes((Int16)messageID).CopyTo(buffer, 0);
            BitConverter.GetBytes((Int16)78).CopyTo(buffer, 2);

            client.Client.Send(buffer);
        }
    }
}

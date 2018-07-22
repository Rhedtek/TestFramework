using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.AccessControl;
using System.Security.Principal;
using ATGUI.DatabaseObjects;

namespace ATGUI
{
    public class ConfigurationEntry
    {
        public string ConfigurationName { get; set; }
        public int ConfigurationID { get; set; }
    }

    /// <summary>
    /// Interaction logic for CreateTestSuite.xaml
    /// </summary>
    public partial class CreateTestSuite : Window
    {
        //private List<ConfigurationEntry> mConfigurations = new List<ConfigurationEntry>();
        List<ConfigurationData> mSelectedConfigurations = new List<ConfigurationData>();

        private TestPackageData mPackage;
        private bool mScheduled = false;
        private int mOperatingSystemID;

        public CreateTestSuite()
        {
            InitializeComponent();
            TestSuiteNameTB_TextChanged(null, null);
            NumberOfRunsTB_TextChanged(null, null);

            FileDrop.Visibility = Visibility.Hidden;
            DownloadLinkLL.Visibility = Visibility.Hidden;
            DragDropLL.Visibility = Visibility.Hidden;
            DragDropFileLL.Visibility = Visibility.Hidden;
            DownloadLinkCB.Visibility = Visibility.Hidden;
        }

        public bool Initialize(TestPackageData testPackage, bool scheduled, int operatingSystemID)
        {
            mScheduled = scheduled;
            mOperatingSystemID = operatingSystemID;

            if (mScheduled == false)
            {
                SchedulingGB.Visibility = Visibility.Hidden;
            }

            try
            {
                mPackage = testPackage;
                PackageNameLabel.Content = mPackage.TestPackageName;

                // Get all license keys
                if (string.IsNullOrEmpty(mPackage.LicenseKey))
                {
                    var result = LicenseKeyData.Select();
                    foreach (var key in result)
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = key.Name;
                        item.Tag = key.LicenseKeyString;
                        item.ToolTip = key.Description;
                        LicenseKeyCB.Items.Add(item);
                    }
                }
                else
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = mPackage.LicenseKey;
                    item.Tag = mPackage.LicenseKey;
                    item.ToolTip = "From TestPackage.json";
                    LicenseKeyCB.Items.Add(item);
                    LicenseKeyCB.SelectedItem = item;
                }

                var DownloadLinks = DownloadLinkData.Select(mOperatingSystemID);
                foreach (var DownloadLink in DownloadLinks)
                {
                    DownloadLinkCB.Items.Add(new ComboBoxItem()
                    {
                        Content = DownloadLink.DownloadLink,
                        Tag = DownloadLink.DownloadLink,
                        ToolTip = DownloadLink.DownloadLinkDescription
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }

            return true;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TestSuiteNameTB.Text))
            {
                MessageBox.Show("Test Suite must have a name.");
                return;
            }

            bool result = SubmitData();
            if (result == true)
            {
                this.Close();
            }
        }

        private bool Check()
        {
            int result;

            if (string.IsNullOrEmpty(NumberOfRunsTB.Text) == true)
            {
                MessageBox.Show("Please specify the number of runs.");
                return false;
            }

            if (int.TryParse(NumberOfRunsTB.Text, out result) == false || result <= 0)
            {
                MessageBox.Show("Number of runs must be a positive interger value.");
                NumberOfRunsTB.Text = string.Empty;
                return false;
            }

            if (string.IsNullOrEmpty(MaxRunningTB.Text) == false && int.TryParse(MaxRunningTB.Text, out result) == false || result <= 0)
            {
                MessageBox.Show("Max number must be a positive interger value.");
                MaxRunningTB.Text = string.Empty;
                return false;
            }

            if (mSelectedConfigurations.Count == 0)
            {
                MessageBox.Show("Please select one or more configurations.");
                return false;
            }

            if (LicenseKeyCB.SelectedItem == null)
            {
                MessageBox.Show("Please select a license key that you want to use.");
                return false;
            }

            if (ReleaseCB.SelectedItem == null)
            {
                MessageBox.Show("Please select a version of APP to use.");
                return false;
            }

            var selectedVersion = (ComboBoxItem)ReleaseCB.SelectedItem;
            var selectedDownloadLink = (ComboBoxItem)DownloadLinkCB.SelectedItem;
            if (selectedDownloadLink == null && selectedVersion.Tag.ToString() == "latest")
            {
                MessageBox.Show("Please select a DownloadLink that you want to use.");
                return false;
            }

            if (selectedVersion.Tag.ToString() == "dragdrop" && string.IsNullOrEmpty((string)DragDropFileLL.Content))
            {
                MessageBox.Show("Please drag and drop a file you want to use.");
                return false;
            }

            if (string.IsNullOrEmpty(TestSuiteNameTB.Text) == true)
            {
                MessageBox.Show("TestSuite name cannot be empty.");
                return false;
            }

            int priority = 10;
            if (int.TryParse(PriorityTB.Text, out priority) == false)
            {
                MessageBox.Show("Value for 'Priority' is not an integer.");
                return false;
            }
            if (priority < 1 || priority > 10)
            {
                MessageBox.Show("Priority should be between 1 and 10.");
                return false;
            }

            return true;
        }

        private bool CheckSchedule()
        {
            if (string.IsNullOrEmpty(SunTB.Text) == true &&
                string.IsNullOrEmpty(MonTB.Text) == true &&
                string.IsNullOrEmpty(TueTB.Text) == true &&
                string.IsNullOrEmpty(WedTB.Text) == true &&
                string.IsNullOrEmpty(ThuTB.Text) == true &&
                string.IsNullOrEmpty(FriTB.Text) == true &&
                string.IsNullOrEmpty(SatTB.Text) == true)
            {
                MessageBox.Show("It doesn't look like you specified much of a schedule..?");
                return false;
            }
            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool SubmitData()
        {
            try
            {
                bool check = Check();
                if (check == false)
                {
                    return false;
                }

                if (mScheduled)
                {
                    check = CheckSchedule();
                    if (check == false)
                    {
                        return false;
                    }
                }

                int priority = 10;
                int.TryParse(PriorityTB.Text, out priority);

                int? maxRunning = null;
                if (string.IsNullOrEmpty(MaxRunningTB.Text) == false)
                {
                    maxRunning = int.Parse(MaxRunningTB.Text);
                }

                // Create TestSuite first
                int testSuiteID;
                if (mScheduled == false)
                {
                    testSuiteID = TestSuiteData.Create(TestSuiteNameTB.Text, DescriptionTB.Text, mPackage.TestPackageID, priority, maxRunning, mOperatingSystemID);
                }
                else
                {
                    testSuiteID = TestSuitePlanData.Create(TestSuiteNameTB.Text, DescriptionTB.Text, mPackage.TestPackageID, priority, maxRunning, mOperatingSystemID);
                }

                string dragDropFile = string.Empty;

                var selectedVersion = (ComboBoxItem)ReleaseCB.SelectedItem;
                if (selectedVersion.Tag.ToString() == "dragdrop")
                {
                    dragDropFile = DragDropFileLL.Content.ToString();
                    dragDropFile = HandleDragDropFile(dragDropFile, testSuiteID);
                    dragDropFile = dragDropFile.Replace("C:\\", string.Empty);
                }

                int numberOfRuns = int.Parse(NumberOfRunsTB.Text);
                var selectedLicense = (ComboBoxItem)LicenseKeyCB.SelectedItem;
                var selectedDownloadLink = (ComboBoxItem)DownloadLinkCB.SelectedItem;
                var DownloadLink = (selectedDownloadLink == null ? string.Empty : (string)selectedDownloadLink.Tag);

                for (int index = 0; index < numberOfRuns; ++index)
                {
                    foreach (var config in mSelectedConfigurations)
                    {
                        if (config.ConfigurationID == -1) continue; // -1 = all configurations
                        if (config.ConfigurationID == 25) continue; // 25 = first available configuration

                        // Create TestJob
                        int testJobID;
                        if (mScheduled == false)
                        {
                            testJobID = TestJobData.Create(testSuiteID, mPackage.TestPackageID, config.ConfigurationID, (string)selectedLicense.Tag,
                                DownloadLink.ToLower(), priority, (string)selectedVersion.Tag, dragDropFile);

                            // Create test instances
                            TestJobCommandData.Create(testJobID);
                        }
                        else
                        {
                            testJobID = TestJobPlanData.Create(testSuiteID, mPackage.TestPackageID, config.ConfigurationID, (string)selectedLicense.Tag,
                                DownloadLink.ToLower(), priority, (string)selectedVersion.Tag, dragDropFile);
                        }
                    }
                }

                // Add schedule
                if (mScheduled == true)
                {
                    int testPlanScheduleID = TestSuiteScheduleData.Create(testSuiteID);

                    HandleScheduleTextBox(SunTB, testPlanScheduleID, 1);
                    HandleScheduleTextBox(MonTB, testPlanScheduleID, 2);
                    HandleScheduleTextBox(TueTB, testPlanScheduleID, 3);
                    HandleScheduleTextBox(WedTB, testPlanScheduleID, 4);
                    HandleScheduleTextBox(ThuTB, testPlanScheduleID, 5);
                    HandleScheduleTextBox(FriTB, testPlanScheduleID, 6);
                    HandleScheduleTextBox(SatTB, testPlanScheduleID, 7);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during submit: " + ex);
            }

            return true;
        }

        private void HandleScheduleTextBox(TextBox tb, int testPlanScheduleID, int weekDayID)
        {
            int hours = 0;
            int minutes = 0;
            if (GetHourAndMinute(tb.Text, out hours, out minutes) == true)
            {
                TestSuiteScheduleEntryData.Create(testPlanScheduleID, weekDayID, hours, minutes);
            }
        }

        private bool GetHourAndMinute(string text, out int hours, out int minutes)
        {
            hours = minutes = 0;

            if (string.IsNullOrEmpty(text) == true)
            {
                return false;
            }

            int number = int.Parse(text);
            minutes = number % 100;
            hours = number / 100;
            return true;
        }

        private void TestSuiteNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TestSuiteNameTB.Text) == true)
            {
                TestSuiteNameTB.Background = Brushes.Orange;
            }
            else
            {
                TestSuiteNameTB.Background = Brushes.White;
            }
        }

        private void ReleaseCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReleaseCB.SelectedItem == null)
            {
                DownloadLinkCB.IsEnabled = true;
                ReleaseCB.Background = Brushes.Orange;
                return;
            }

            ReleaseCB.Background = (new BrushConverter().ConvertFromString("#FFDDDDDD") as SolidColorBrush);

            var release = ReleaseCB.SelectedItem as ComboBoxItem;
            if (release.Tag.ToString() == "latest")
            {
                DownloadLinkCB.Visibility = Visibility.Visible;
                DownloadLinkLL.Visibility = Visibility.Visible;
            }
            else
            {
                DownloadLinkCB.Visibility = Visibility.Hidden;
                DownloadLinkLL.Visibility = Visibility.Hidden;
            }

            if (release.Tag.ToString() == "dragdrop")
            {
                FileDrop.Visibility = Visibility.Visible;
                DragDropLL.Visibility = Visibility.Visible;
                DragDropFileLL.Visibility = Visibility.Visible;
            }
            else
            {
                FileDrop.Visibility = Visibility.Hidden;
                DragDropLL.Visibility = Visibility.Hidden;
                DragDropFileLL.Visibility = Visibility.Hidden;
                DragDropFileLL.Content = string.Empty;
            }
        }

        private void NumberOfRunsTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(NumberOfRunsTB.Text) == true)
            {
                NumberOfRunsTB.Background = Brushes.Orange;
            }
            else
            {
                int result = 0;
                if (int.TryParse(NumberOfRunsTB.Text, out result) == false)
                {
                    NumberOfRunsTB.Background = Brushes.Red;
                }
                else
                {
                    NumberOfRunsTB.Background = Brushes.White;
                }
            }
        }

        private void MaxRunningTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(MaxRunningTB.Text) == true)
            {
                MaxRunningTB.Background = Brushes.White;
            }
            else
            {
                int result = 0;
                if (int.TryParse(MaxRunningTB.Text, out result) == false)
                {
                    MaxRunningTB.Background = Brushes.Red;
                }
                else
                {
                    MaxRunningTB.Background = Brushes.White;
                }
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FileDrop_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    DragDropFileLL.Content = files[0];
                }
            }
        }

        private string HandleDragDropFile(string path, int testSuiteID)
        {
            string filename = System.IO.Path.GetFileName(path);
            string newPath = System.IO.Path.Combine("C:\\TestPackages\\_TestSuiteData", testSuiteID.ToString());
            Directory.CreateDirectory(newPath);

            string fullPath = System.IO.Path.Combine(newPath, filename);

            File.Copy(path, fullPath);

            MessageBox.Show("Copied " + path + " to " + fullPath);

            DirectorySecurity sec = Directory.GetAccessControl(newPath);
            // Using this instead of the "Everyone" string means we work on non-English systems.
            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            Directory.SetAccessControl(newPath, sec);

            return newPath;
        }

        private void LabCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ConfigSelect_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationSelector selector = new ConfigurationSelector();
            selector.Init(mOperatingSystemID, mPackage.AutomationLabID, mSelectedConfigurations);
            selector.ShowDialog();

            string buttonContent = "Select";
            Brush color = Brushes.Orange;

            if (mSelectedConfigurations.Count > 0)
            {
                buttonContent = "Selected: " + mSelectedConfigurations.Count;
                color = (new BrushConverter().ConvertFromString("#FFDDDDDD") as SolidColorBrush);
            }

            ConfigSelect.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(delegate ()
                {
                    ConfigSelect.Content = buttonContent;
                    ConfigSelect.Background = color;
                }));
        }
    }
}

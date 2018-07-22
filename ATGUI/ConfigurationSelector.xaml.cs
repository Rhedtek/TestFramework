using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using ATGUI.DatabaseObjects;

namespace ATGUI
{
    /// <summary>
    /// Interaction logic for ConfigurationSelector.xaml
    /// </summary>
    public partial class ConfigurationSelector : Window
    {
        List<ConfigurationData> mSelectedConfigurations;
        ObservableCollection<ConfigurationData> mOriginalList = new ObservableCollection<ConfigurationData>();
        List<ConfigurationData> mSource = new List<ConfigurationData>();
        private ListSortDirection currentDir = ListSortDirection.Descending;
        private string currentSortedColumn = string.Empty;
        private int mOperatingSystemID;

        public ConfigurationSelector()
        {
            InitializeComponent();
        }

        public void Init(int operatingSystemID, int labID, List<ConfigurationData> configurations)
        {
            mOperatingSystemID = operatingSystemID;
            mSelectedConfigurations = configurations;
            Configurations.ItemsSource = mOriginalList;

            // Fetch data
            mSource = ConfigurationData.Select(mOperatingSystemID, labID);

            // Copy Selected falgs from previously selected configurations
            foreach (var config in mSelectedConfigurations)
            {
                if (config.Selected == true)
                {
                    var item = mSource.FirstOrDefault(e => e.ConfigurationID == config.ConfigurationID);
                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }
            }

            bool allSelected = mSource.All(e => e.Selected == true);
            SelectAll.IsChecked = allSelected;

            Refresh();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                int tag = (int)(checkBox.Tag);
                var configuration = mOriginalList.First(ee => ee.ConfigurationID == tag);
                configuration.Selected = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                int tag = (int)(checkBox.Tag);
                var configuration = mOriginalList.First(ee => ee.ConfigurationID == tag);
                configuration.Selected = false;
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
            Refresh();
        }

        private void SortData()
        {
            if (currentSortedColumn == ConfigurationNameColumnHeader.Name)
            {
                SortList(info => info.ConfigurationName);
            }
            else
            {
                SortList(info => info.Selected);
            }
        }

        private void SortList<T>(Func<ConfigurationData, T> func)
        {
            if (currentDir == ListSortDirection.Ascending)
            {
                mSource = mSource.OrderBy(func).ToList();
            }
            else
            {
                mSource = mSource.OrderByDescending(func).ToList();
            }
        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var data in mOriginalList)
            {
                data.Selected = true;
            }
            Refresh();
        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var data in mOriginalList)
            {
                data.Selected = false;
            }
            Refresh();
        }

        // Easiest way to refresh it to add all elements back in
        private void Refresh()
        {
            mOriginalList.Clear();
            mOriginalList.AddRange(mSource);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Nothing to do
            this.Close();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            mSelectedConfigurations.Clear();
            foreach (var data in mOriginalList)
            {
                if (data.Selected == true)
                {
                    mSelectedConfigurations.Add(data);
                }
            }
            this.Close();
        }
    }
}

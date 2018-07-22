using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ATGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<TabItem> allTabItems = new ObservableCollection<TabItem>();
        private Dictionary<string, TabItem> mNameToTabMapping = new Dictionary<string, TabItem>();
        private Dictionary<Page, TabItem> mPageToTabMapping = new Dictionary<Page, TabItem>();
        private TestSuites mTestSuitesPage = new TestSuites();
        private List<TabItem> mHistory = new List<TabItem>();
        private Packages mPackages = new Packages();
        private VMInstances mVMPage = new VMInstances();
        private TestJobs mJobs = new TestJobs();
        private TestJobCommands mTestJobCommands = new TestJobCommands();

        public MainWindow()
        {
            InitializeComponent();

            Thread t = new Thread(new ThreadStart(Start));
            t.Start();
        }

        internal void SetOperatingSystem(int ID)
        {
            mPackages.Refresh();
            mVMPage.Refresh(ID);
            mTestSuitesPage.Refresh(ID);
            mJobs.Refresh(ID);
            mTestJobCommands.Refresh(ID);
        }

        internal void SetLab(int ID)
        {
            mPackages.Refresh();
        }

        private void Start()
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(delegate () { Init(); }));
        }

        private void Init()
        {
            try
            {
                TabControl.ItemsSource = allTabItems;

                mPackages.Initialize(this);
                AddTab(mPackages, "Test Packages", true);

                mTestSuitesPage.Initialize(this);
                AddTab(mTestSuitesPage, "Test Suites", false);

                mJobs.Initialize(this);
                AddTab(mJobs, "Test Jobs", false);

                mTestJobCommands.Initialize(null, null, this);
                AddTab(mTestJobCommands, "Test Job Commands", false);

                mVMPage.Initialize(this);
                AddTab(mVMPage, "VMs", false);

                TestSuitePlan testSuitePlan = new TestSuitePlan();
                AddTab(testSuitePlan, "Schedules", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedItem = e.AddedItems[0] as TabItem;
                if (selectedItem != null)
                {
                    if (mHistory.Contains(selectedItem))
                    {
                        mHistory.Remove(selectedItem);
                    }
                    mHistory.Add(selectedItem);
                    //MessageBox.Show("Count: " + mHistory.Count + "; Last: " + mHistory.Last().Header + "; count " + e.AddedItems.Count);
                }
            }
        }

        internal TabItem AddTab(Page page, string title, bool selected = false, int position = -1)
        {
            if (mNameToTabMapping.ContainsKey(title) == true)
            {
                var tab = mNameToTabMapping[title];
                tab.IsSelected = true;
                return tab;
            }

            TabItem item = new TabItem();
            Frame frame = new Frame();
            frame.Content = page;
            item.Content = frame;
            item.Header = title;
            item.ToolTip = "Double click the tab to refresh the content of the page.";
            item.Tag = page;
            item.IsSelected = selected;
            allTabItems.Add(item);
            mNameToTabMapping.Add(title, item);
            mPageToTabMapping.Add(page, item);
            return item;
        }

        internal void RemoveTabItem(Page page)
        {
            if (mPageToTabMapping.ContainsKey(page))
            {
                var tabItem = mPageToTabMapping[page];
                allTabItems[0].IsSelected = true;

                allTabItems.Remove(tabItem);
                mNameToTabMapping.Remove((string)tabItem.Header);

                var last = mHistory[mHistory.Count - 2];
                mHistory.Remove(tabItem);

                if (mHistory.Count > 0)
                {
                    //MessageBox.Show("Selecting " + last.Header);
                    last.IsSelected = true;
                }
            }
        }
    }
}

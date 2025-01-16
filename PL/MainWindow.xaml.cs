using PL.Call;
using PL.Volunteer;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        private static bool isManagerLoggedIn = false;
        private static bool isHandleCalls = false;
        private static bool isHandleVolunteer = false;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public class helper
        {
            public int index { get; set; }
            public BO.MyCallStatus value { get; set; }
        }



        private int idManager;
        public MainWindow(int id)
        {
            idManager= id;
            InitializeComponent();
            DataContext = this;
            var callAmounts = s_bl.Call.CallAmount();
            var statusList = callAmounts
                .Select((count, index) => new helper
                {
                    index = count,
                    value = (BO.MyCallStatus)index
                })
                .ToList();

            StatusList = statusList;

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }
         public helper selectedValue { get; set; }
        public IEnumerable<helper> StatusList
        {
            get { return (IEnumerable<helper>)GetValue(StatusListProperty); }
            set { SetValue(StatusListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusListProperty =
            DependencyProperty.Register("StatusList", typeof(IEnumerable<helper>), typeof(MainWindow), new PropertyMetadata(null));

        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }
        private void configObserver()
        {
            MaxRiskRange = s_bl.Admin.GetRiskRange();
        }
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        public TimeSpan MaxRiskRange
        {
            get { return (TimeSpan)GetValue(MaxRiskRangeProperty); }
            set { SetValue(MaxRiskRangeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));
        public static readonly DependencyProperty MaxRiskRangeProperty =
    DependencyProperty.Register("MaxRiskRange", typeof(TimeSpan), typeof(MainWindow));
      private void MainWindow_Loaded(object sender, EventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            MaxRiskRange = s_bl.Admin.GetRiskRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }
        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            isManagerLoggedIn = false; //Release the manager variable when a window is closed
        }
        //private void OpenManagerWindow()
        //{
        //    MainWindow mainW = new MainWindow();
        //    mainW.Closed += MainWindow_Closed; // Add the Closed event to the admin window
        //    mainW.Show();
        //}

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.Clock.Day);
        }

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.Clock.Minute);

        }

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.Clock.Hour);

        }

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.Clock.Month);

        }

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.Clock.Year);

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskRange(MaxRiskRange);

        }

        private void btnInitDB_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to initialize the database?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Close all open windows except the main window
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                    {
                        window.Close();
                    }
                }

                // Call the initialization method
                s_bl.Admin.Initialization();
                // Change cursor to wait cursor
                Mouse.OverrideCursor =null;
            }
        }

        private void btnResetDB_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to reset the database?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Close all open windows except the main window
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                    {
                        window.Close();
                    }
                }
                // Call the reset method
                s_bl.Admin.Reset();
                // Change cursor to wait cursor

                Mouse.OverrideCursor = null;

            }
        }

        private void btnHandleVolunteers_Click(object sender, RoutedEventArgs e)
        {
            if (isHandleVolunteer)
            {
                 MessageBox.Show("Volunteers screen window is already open", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
             }
             else
             {
                    isHandleVolunteer = true;
                new VolunteerListWindow().Show();
              }
        }
        private void btnHandleCalls_Click(object sender, RoutedEventArgs e)
        {
            if (isHandleCalls)
            {
                MessageBox.Show("Calls screen window is already open", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else 
            { 

                isHandleCalls = true;
                new CallListWindow(idManager).Show();
            }
        }
        private void DataGrid_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
           new CallListWindow(idManager,selectedValue.value).Show();
        }
    }
}
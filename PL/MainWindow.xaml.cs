using PL.Call;
using PL.Volunteer;
using System;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool isHandleCalls = false;
        private static bool isHandleVolunteer = false;

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Helper class for displaying call status.
        /// </summary>
        public class Helper
        {
            public int index { get; set; }
            public BO.MyCallStatus value { get; set; }
        }

        private int idManager;

        /// <summary>
        /// Constructor for MainWindow. Initializes components and loads call amounts.
        /// </summary>
        /// <param name="id">ID of the manager.</param>
        public MainWindow(int id)
        {
            idManager = id;
            selectedValue = new Helper(); // Initialize selectedValue
            InitializeComponent();
            updateCallAmount();
        }
        private void updateCallAmount()
        {
            var callAmounts = s_bl.Call.CallAmount();
            var statusListAmount = callAmounts
                .Select((count, index) => new Helper
                {
                    index = count,
                    value = (BO.MyCallStatus)index
                })
                .ToList();

            Dispatcher.Invoke(() =>
            {
                StatusList = statusListAmount;
            });
        }


        /// <summary>
        /// Gets or sets the selected value.
        /// </summary>
        public Helper selectedValue { get; set; }

        /// <summary>
        /// Gets or sets the status list.
        /// </summary>
        public IEnumerable<Helper> StatusList
        {
            get { return (IEnumerable<Helper>)GetValue(StatusListProperty); }
            set { SetValue(StatusListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusList. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusListProperty =
            DependencyProperty.Register("StatusList", typeof(IEnumerable<Helper>), typeof(MainWindow), new PropertyMetadata(null));

        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusList. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow), new PropertyMetadata(1000));
        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public bool IsSimulatorRunning
        {
            get { return (bool)GetValue(IsSimulatorRunningProperty); }
            set
            {
                SetValue(IsSimulatorRunningProperty, value);
                OnPropertyChanged(nameof(IsSimulatorRunning));
                OnPropertyChanged(nameof(ButtonContent)); 
            }
        }

        public string ButtonContent
        {
            get { return IsSimulatorRunning ? "Stop Simulator" : "Start Simulator"; }
            set { }
        }

        private void ToggleSimulator_Click(object sender, RoutedEventArgs e)
        {
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }
            else
            {
                s_bl.Admin.StartSimulator(Interval);
                IsSimulatorRunning = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Observer method to update the current time.
        /// </summary>
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void ClockObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)

                _observerOperation = Dispatcher.BeginInvoke(() =>
                { CurrentTime = s_bl.Admin.GetClock(); });
             
        }

        /// <summary>
        /// Observer method to update the maximum risk range.
        /// </summary>
        private volatile DispatcherOperation? _observerOperation2 = null; //stage 7
        private void configObserver()
        {
            if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
                _observerOperation2 = Dispatcher.BeginInvoke(() =>{ MaxRiskRange = s_bl.Admin.GetRiskRange(); });
        }


        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum risk range.
        /// </summary>
        public TimeSpan MaxRiskRange
        {
            get { return (TimeSpan)GetValue(MaxRiskRangeProperty); }
            set { SetValue(MaxRiskRangeProperty, value); }
        }

        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));
        public static readonly DependencyProperty MaxRiskRangeProperty =
            DependencyProperty.Register("MaxRiskRange", typeof(TimeSpan), typeof(MainWindow));

        /// <summary>
        /// Event handler for window loaded event. Initializes current time and risk range, adds observers.
        /// </summary>
        private void MainWindow_Loaded(object sender, EventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            MaxRiskRange = s_bl.Admin.GetRiskRange();
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Admin.AddConfigObserver(updateCallAmount);
            s_bl.Call.AddObserver(updateCallAmount);
        }

        /// <summary>
        /// Event handler for window closed event. Removes observers.
        /// </summary>
        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            s_bl.Admin.StopSimulator();
            IsSimulatorRunning = false;
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Admin.RemoveConfigObserver(updateCallAmount);
            s_bl.Call.RemoveObserver(updateCallAmount);

        }

        /// <summary>
        /// Event handler for advancing the clock by one day.
        /// </summary>
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.Clock.Day);
        }

        /// <summary>
        /// Event handler for advancing the clock by one minute.
        /// </summary>
        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.Clock.Minute);
        }

        /// <summary>
        /// Event handler for advancing the clock by one hour.
        /// </summary>
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.Clock.Hour);
        }

        /// <summary>
        /// Event handler for advancing the clock by one month.
        /// </summary>
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.Clock.Month);
        }

        /// <summary>
        /// Event handler for advancing the clock by one year.
        /// </summary>
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.Clock.Year);
        }

        /// <summary>
        /// Event handler for updating the maximum risk range.
        /// </summary>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskRange(MaxRiskRange);
        }

        /// <summary>
        /// Event handler for initializing the database.
        /// </summary>
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
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Event handler for resetting the database.
        /// </summary>
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
                updateCallAmount();
                // Change cursor to wait cursor
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Event handler for handling volunteers.
        /// </summary>
        private void btnHandleVolunteers_Click(object sender, RoutedEventArgs e)
        {
            if (isHandleVolunteer)
            {
                MessageBox.Show("Volunteers screen window is already open", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var Volunteer = new VolunteerListWindow();
                Volunteer.Closed += (s, e) => isHandleVolunteer = false;
                Volunteer.Show();
                isHandleVolunteer = true;
            }
        }

        /// <summary>
        /// Event handler for handling calls.
        /// </summary>
        private void btnHandleCalls_Click(object sender, RoutedEventArgs e)
        {
            if (isHandleCalls)
            {
                MessageBox.Show("Calls screen window is already open", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var call = new CallListWindow(idManager);
                call.Closed += (s, e) => isHandleCalls = false;
                call.Show();
                isHandleCalls = true;
            }
        }

        /// <summary>
        /// Event handler for mouse left button up event on the data grid. Opens the call list window.
        /// </summary>
        private void DataGrid_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            new CallListWindow(idManager, selectedValue.value).Show();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

using BO;
using PL.Volunteer;
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
using System.Windows.Threading;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for SelectCallToTreatWindow.xaml
    /// </summary>
    public partial class SelectCallToTreatWindow : Window
    {
        private const string GoogleMapsApiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4";
        public BO.OpenCallInList? SelectedOpenCall { get; set; }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.MyCallType FilterByType { get; set; } = BO.MyCallType.None;

        public IEnumerable<BO.OpenCallInList> OpenCallList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallListProperty); }
            set { SetValue(OpenCallListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenCallListProperty =
            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(SelectCallToTreatWindow), new PropertyMetadata(null));
        public String Description
        {
            get { return (String)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(String), typeof(SelectCallToTreatWindow), new PropertyMetadata(string.Empty));


        public BO.Call? Call
        {
            get { return (BO.Call?)GetValue(CallProperty); }
            set { SetValue(CallProperty, value); }
        }
        public static readonly DependencyProperty CallProperty =
        DependencyProperty.Register("Call", typeof(BO.Call), typeof(SelectCallToTreatWindow), new PropertyMetadata(null));
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(SelectCallToTreatWindow), new PropertyMetadata(null));
        public string MapSource
        {
            get { return (string)GetValue(MapSourceProperty); }
            set { SetValue(MapSourceProperty, value); }
        }
        public static readonly DependencyProperty MapSourceProperty =
        DependencyProperty.Register("MapSource", typeof(string), typeof(SelectCallToTreatWindow), new PropertyMetadata(null));

        private int id;

        /// <summary>  
        /// Constructor for SelectCallToTreatWindow. Initializes components and loads volunteer details if ID is provided.  
        /// </summary>  
        /// <param name="idV">ID of the volunteer.</param>  
        public SelectCallToTreatWindow(int idV)
        {
            try
            {
                id = idV;
                InitializeComponent();
                CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id)! : new BO.Volunteer
                {
                    FullName = string.Empty,
                    Phone = string.Empty,
                    Email = string.Empty
                };
                queryCallList(id);
                MapSource = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "mapp.html");
                if (SelectedOpenCall != null)
                    Description = SelectedOpenCall.Description ?? string.Empty;
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
            }
        }

        /// <summary>
        /// Event handler for window loaded event. Adds observer for the call list.
        /// </summary>
        private void CallListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(CurrentVolunteer!.Id, callListObserver);
            MapWebView.NavigationCompleted += async (s, args) =>
            {
                CenterMapOnVolunteer();
            };

            s_bl.Call.AddObserver(callListObserver);
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id)! : new BO.Volunteer
            {
                FullName = string.Empty,
                Phone = string.Empty,
                Email = string.Empty
            };
        }

        /// <summary>
        /// Queries the call list based on the filter by type.
        /// </summary>
        /// <summary>
        /// Queries the list of open calls for the specified volunteer.
        /// Retrieves the details of the volunteer and calls the service to get the open calls, applying a filter if specified.
        /// </summary>
        /// <param name="id">The ID of the volunteer for whom the call list query should be performed.</param>
        private void queryCallList(int id)
        {
            try
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                if (CurrentVolunteer == null)
                    throw new NullReferenceException("CurrentVolunteer is null.");

                Console.WriteLine($"Querying call list for Volunteer ID: {CurrentVolunteer.Id}, Filter: {FilterByType}");

                OpenCallList = FilterByType == BO.MyCallType.None
                    ? s_bl.Call.SortOpenedCalls(CurrentVolunteer.Id, null, null)
                    : s_bl.Call.SortOpenedCalls(CurrentVolunteer.Id, FilterByType, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to query call list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Error in queryCallList: {ex}");
            }
        }


        /// <summary>
        /// Observer method to update the call list.
        /// </summary>
        private volatile DispatcherOperation? _observerOperation = null;
        private void callListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    if (CurrentVolunteer != null)
                        queryCallList(id);
                });
        }

        /// <summary>
        /// Event handler for window closed event. Removes observer for the call list.
        /// </summary>
        private void CallListWindow_Closed(object? sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(callListObserver);
        }

        /// <summary>
        /// Event handler for filter changes in the combo box. Re-queries the call list.
        /// </summary>
        private void cmbFiltedrChanges_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCallList(id);
        }

        /// <summary>
        /// Event handler for selecting a call to treat. Assigns the call to the volunteer and navigates to the volunteer user window.
        /// </summary>
        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.SelectCallToTreat(id, SelectedOpenCall!.Id);
                MessageBox.Show("A call has been selected for treatment", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (BO.BlTemporaryNotAvailableException ex)
            {
                MessageBox.Show($"{ex.Message}\nPlease stop the Simulator and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlInvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
            }
        }

        /// <summary>
        /// Event handler for mouse left button up event on the data grid. Opens the call description window.
        /// </summary>

        private async void CenterMapOnVolunteer()
        {
            if (Call is not null)
            {
                await MapWebView.ExecuteScriptAsync($"initialize({CurrentVolunteer.Latitude}, " +
                    $"{CurrentVolunteer.Longitude}, {Call.Latitude}, " +
                    $"{Call.Longitude}, '{CurrentVolunteer.TypeDistance}');");
            }
            else
            {
                await MapWebView.ExecuteScriptAsync($"initialize({CurrentVolunteer.Latitude}, " +
                    $"{CurrentVolunteer.Longitude}, {CurrentVolunteer.Latitude}, " +
                    $"{CurrentVolunteer.Longitude}, '{CurrentVolunteer.TypeDistance}');");
            }
        }
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null)
                return;
            if (SelectedOpenCall != null)
            {
                Description = SelectedOpenCall.Description ?? string.Empty;
            }
            if (SelectedOpenCall is BO.OpenCallInList openCall)
            {
                Call = s_bl.Call.GetCallDetails(openCall.Id);
                CenterMapOnVolunteer();
            }
        }

        /// <summary>
        /// Event handler for updating the volunteer's address. Updates the address and re-queries the call list.
        /// </summary>
        private void UpdateAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.UpdateVolunteer(id, CurrentVolunteer);
                MessageBox.Show("The address has been updated successfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                queryCallList(id);
                CenterMapOnVolunteer();

            }
            catch (BO.BlInvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlUnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
            }
        }

    }
}

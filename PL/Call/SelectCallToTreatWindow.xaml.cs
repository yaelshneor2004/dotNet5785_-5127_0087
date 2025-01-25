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

        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(SelectCallToTreatWindow), new PropertyMetadata(null));

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
                CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id)! : new BO.Volunteer();
                queryCallList(id );
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
            s_bl.Call.AddObserver(callListObserver);
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id)! : new BO.Volunteer();
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
                } );
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
        private void DataGrid_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            List<string> addresses = OpenCallList.Select(call => call.Address).ToList();
            new CallDescription(CurrentVolunteer.Id,SelectedOpenCall.Description).Show();
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

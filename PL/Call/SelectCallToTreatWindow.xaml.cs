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
                Loaded += CallListWindow_Loaded;
                Closed += CallListWindow_Closed;
                CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id)! : new BO.Volunteer();

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
        }

        /// <summary>
        /// Queries the call list based on the filter by type.
        /// </summary>
        private void queryCallList()
        {
            OpenCallList = (FilterByType == BO.MyCallType.None) ?
                s_bl?.Call.SortOpenedCalls(id, null, null)! :
                s_bl?.Call.SortOpenedCalls(id, FilterByType, null)!;
        }

        /// <summary>
        /// Observer method to update the call list.
        /// </summary>
        private void callListObserver()
        {
            queryCallList();
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
            queryCallList();
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
            new CallDescription(addresses, CurrentVolunteer.Address, SelectedOpenCall.Id).Show();
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
                queryCallList();
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

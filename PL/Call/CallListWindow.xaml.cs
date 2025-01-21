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
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        public BO.CallInList? SelectedCall { get; set; }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.MyCallStatus SortInCallInList { get; set; } = BO.MyCallStatus.None;
        private int idManager;

        /// <summary>
        /// Constructor for CallListWindow.
        /// </summary>
        public CallListWindow(int id, BO.MyCallStatus status = BO.MyCallStatus.None)
        {
            idManager = id;
            SortInCallInList = status;
            InitializeComponent();
        }

        /// <summary>
        /// Property for CallList.
        /// </summary>
        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

        /// <summary>
        /// Event handler for selection changes in combo box.
        /// </summary>
        private void cmbSelectChanges_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCallList();
        }

        /// <summary>
        /// Queries the call list based on the current sorting status.
        /// </summary>
        private void queryCallList()
        {
            CallList = (SortInCallInList == BO.MyCallStatus.None) ?
              s_bl?.Call.GetFilterCallList(BO.MyCallStatus.None)! :
              s_bl?.Call.GetFilterCallList(SortInCallInList)!;
        }

        /// <summary>
        /// Observer method to update the call list when changes occur.
        /// </summary>
        private void callListObserver()
        {
            queryCallList();
        }

        /// <summary>
        /// Event handler for when the window is loaded.
        /// </summary>
        private void CallListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(callListObserver);
        }

        /// <summary>
        /// Event handler for when the window is closed.
        /// </summary>
        private void CallListWindow_Closed(object? sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(callListObserver);
        }

        /// <summary>
        /// Event handler for button click to open a new CallWindow.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow().Show();
        }

        /// <summary>
        /// Event handler for double click on the call list to update a call.
        /// </summary>
        private void UpdateCallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
                new CallWindow(SelectedCall.CallId).Show();
        }

        /// <summary>
        /// Event handler to delete a selected call.
        /// </summary>
        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall == null)
            {
                MessageBox.Show("No call selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this call?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.DeleteCall(SelectedCall.CallId);
                    MessageBox.Show("Call deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BlUnauthorizedAccessException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BlNullPropertyException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
                }
            }
        }

        /// <summary>
        /// Event handler to cancel the assignment of a call.
        /// </summary>
        private void CancalAssignment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedCall.Status == BO.MyCallStatus.InProgress || SelectedCall.Status == BO.MyCallStatus.InProgressAtRisk)
                {
                    s_bl.Call.UpdateCancelTreatment(idManager, SelectedCall.CallId);
                    MessageBox.Show("Call assignment canceled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    queryCallList();
                }
            }
            catch (BO.BlUnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlInvalidOperationException ex)
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
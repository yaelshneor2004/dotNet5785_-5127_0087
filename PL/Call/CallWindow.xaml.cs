using BO;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Gets or sets the current call.
        /// </summary>
        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the call type.
        /// </summary>
        public BO.MyCallType CallType { get; set; } = BO.MyCallType.None;

        /// <summary>
        /// Gets or sets the finish type.
        /// </summary>
        public BO.MyFinishType FinishType { get; set; } = BO.MyFinishType.None;

        /// <summary>
        /// Gets or sets the call status.
        /// </summary>
        public BO.MyCallStatus CallStatus { get; set; } = BO.MyCallStatus.None;

        /// <summary>
        /// Gets or sets the text of the button.
        /// </summary>
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Constructor for CallWindow. Initializes components and loads call details if ID is provided.
        /// </summary>
        /// <param name="id">ID of the call (default is 0).</param>
        public CallWindow(int id = 0)
        {
            try
            {
                ButtonText = id == 0 ? "Add" : "Update";
                InitializeComponent();
                CurrentCall = (id != 0) ? s_bl.Call.GetCallDetails(id)! : new BO.Call()
                {
                    Id = 0,
                    Type = BO.MyCallType.None,
                    Address = "",
                    Latitude = 0,
                    Longitude = 0,
                    StartTime = s_bl.Admin.GetClock(),
                    MaxEndTime = s_bl.Admin.GetClock(),
                    Description = "",
                    Status = BO.MyCallStatus.Open,
                    Assignments = null
                };
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
        /// Event handler for Add/Update button click event. Adds or updates call details.
        /// </summary>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCall == null)
            {
                MessageBox.Show("Call details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ButtonText == "Add")
            {
                s_bl.Call.AddCall(CurrentCall);
                MessageBox.Show("Call added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                try
                {
                    s_bl.Call.UpdateCall(CurrentCall);
                    MessageBox.Show("Call updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
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

        /// <summary>
        /// Event handler for window loaded event. Adds observer for the current call.
        /// </summary>
        private void CallWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCall!.Id != 0)
                s_bl.Call.AddObserver(CurrentCall.Id, CallObserver);
        }

        /// <summary>
        /// Event handler for window closed event. Removes observer for the current call.
        /// </summary>
        private void CallWindow_Closed(object? sender, EventArgs e)
        {
            if (CurrentCall!.Id != 0)
                s_bl.Call.RemoveObserver(CurrentCall.Id, CallObserver);
        }

        /// <summary>
        /// Observer method to update the current call details.
        /// </summary>
        private volatile DispatcherOperation? _observerOperation = null; //stage 7 

        private void CallObserver()
        {
            try
            {
                if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                    _observerOperation = Dispatcher.BeginInvoke(() =>
                    {

                        int id = CurrentCall!.Id;
                        CurrentCall = null;
                        CurrentCall = s_bl.Call.GetCallDetails(id);
                    });
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

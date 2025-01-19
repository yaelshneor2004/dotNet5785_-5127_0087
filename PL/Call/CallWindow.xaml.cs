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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));
        public BO.MyCallType CallType { get; set; } = BO.MyCallType.None;
        public BO.MyFinishType FinishType { get; set; }=BO.MyFinishType.None;
        public BO.MyCallStatus CallStatus { get; set; } = BO.MyCallStatus.None;
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(string.Empty));

        public CallWindow(int id=0)
        {
            try
            {
                ButtonText = id == 0 ? "Add" : "Update";
                InitializeComponent();
                Loaded += CallWindow_Loaded;
                Closed += CallWindow_Closed;
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
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
            }

        }
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

        private void CallWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCall!.Id != 0)
                s_bl.Call.AddObserver(CurrentCall.Id, CallObserver);
        }

        private void CallWindow_Closed(object? sender, EventArgs e)
        {
            if (CurrentCall!.Id != 0)
                s_bl.Call.RemoveObserver(CurrentCall.Id, CallObserver);
        }
        private void CallObserver()
        {
            try
            {
                int id = CurrentCall!.Id;
                CurrentCall = null;
                CurrentCall = s_bl.Call.GetCallDetails(id);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
            }
        }
    }
}

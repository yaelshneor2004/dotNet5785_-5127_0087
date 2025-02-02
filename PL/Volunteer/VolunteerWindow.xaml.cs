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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        public BO.MyCallType CallType { get; set; } = BO.MyCallType.None;
        public BO.MyTypeDistance TypeDistance { get; set; } = BO.MyTypeDistance.None;
        public BO.MyRole Role { get; set; } = BO.MyRole.None;
        public BO.MyCallStatusByVolunteer CallStatus { get; set; } = BO.MyCallStatusByVolunteer.None;
        private int id = 0;
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Constructor for VolunteerWindow. Initializes the window and sets up event handlers.
        /// </summary>
        /// <param name="id">The ID of the volunteer. If 0, a new volunteer is being added.</param>
        public VolunteerWindow(int idV = 0)
        {
            try {
                id = idV;
                ButtonText = id == 0 ? "Add" : "Update";
                InitializeComponent();
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
        /// Event handler for the Loaded event. Adds an observer for the current volunteer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void VolunteerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, VolunteerObserver);
        }

        /// <summary>
        /// Event handler for the Closed event. Removes the observer for the current volunteer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void VolunteerWindow_Closed(object? sender, EventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, VolunteerObserver);
        }

        /// <summary>
        /// Event handler for the Add/Update button click event. Adds or updates the volunteer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentVolunteer == null)
                {
                    MessageBox.Show("Volunteer details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                    MessageBox.Show("Volunteer added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteer(id, CurrentVolunteer);
                    MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
            catch (BO.BlNullPropertyException ex)
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
            catch (BO.BlUnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlTemporaryNotAvailableException ex)
            {
                MessageBox.Show($"{ex.Message}\nPlease stop the Simulator and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
            }
       
        }


        /// <summary>
        /// Observer method for the volunteer. Updates the current volunteer details.
        /// </summary>
        private volatile DispatcherOperation? _observerOperation = null; //stage 7 

        private void VolunteerObserver()
        {
            try
            {
                if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                    _observerOperation = Dispatcher.BeginInvoke(() =>
                    {
                        int id = CurrentVolunteer!.Id;
                        CurrentVolunteer = null;
                        CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
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

using PL.Call;
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

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerUserWindow.xaml
/// </summary>
public partial class VolunteerUserWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private const string GoogleMapsApiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4";

    public string MapSource
    {
        get { return (string)GetValue(MapSourceProperty); }
        set { SetValue(MapSourceProperty, value); }
    }
    public static readonly DependencyProperty MapSourceProperty =
    DependencyProperty.Register("MapSource", typeof(string), typeof(VolunteerUserWindow), new PropertyMetadata(null));

    public BO.Volunteer? CurrentVolunteer
    {
        get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerUserWindow), new PropertyMetadata(null));

    public BO.Call? CurrentCall
    {
        get { return (BO.Call?)GetValue(CurrentCallProperty); }
        set { SetValue(CurrentCallProperty, value); }
    }

    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(VolunteerUserWindow), new PropertyMetadata(null));

    public BO.MyTypeDistance TypeDistance { get; set; } = BO.MyTypeDistance.None;
    public BO.MyRole Role { get; set; } = BO.MyRole.None;
    private int id = 0;

    /// <summary>
    /// Constructor for VolunteerUserWindow.
    /// </summary>
    public VolunteerUserWindow(int id)
    {
        try
        {
            InitializeComponent();
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id)! : new BO.Volunteer();
            CurrentCall = CurrentVolunteer.CurrentCall != null ? s_bl.Call.GetCallDetails(CurrentVolunteer.CurrentCall.CallId)! : new BO.Call();
            MapSource = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "mapp.html");

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
    /// Event handler for updating volunteer details.
    /// </summary>
    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer == null)
        {
            MessageBox.Show("Volunteer details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        try
        {
            s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
            MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (BO.BlTemporaryNotAvailableException ex)
        {
            MessageBox.Show($"{ex.Message}\nPlease stop the Simulator and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        catch (Exception ex)
        {
            MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
        }
    }

    /// <summary>
    /// Event handler for viewing call history.
    /// </summary>
    private void btnCallHistory_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerCallHistoryWindow(CurrentVolunteer.Id).Show();
    }

    /// <summary>
    /// Event handler for selecting a call to treat.
    /// </summary>
    private void btnSelectCall_Click(object sender, RoutedEventArgs e)
    {
        new SelectCallToTreatWindow(CurrentVolunteer.Id).Show();
        //Close();
    }

    /// <summary>
    /// Event handler for canceling the current call.
    /// </summary>
    private void btnCancelCall_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentCall == null)
        {
            MessageBox.Show("Call details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        try
        {
            s_bl.Call.UpdateCancelTreatment(CurrentVolunteer.Id, CurrentCall.Id);
            MessageBox.Show("Treatment cancellation successfully canceled", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (BO.BlTemporaryNotAvailableException ex)
        {
            MessageBox.Show($"{ex.Message}\nPlease stop the Simulator and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        catch (Exception ex)
        {
            MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
        }
    }

    /// <summary>
    /// Event handler for finishing the current call.
    /// </summary>
    private void btnFinishCall_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentCall == null)
        {
            MessageBox.Show("Call details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        try
        {
            s_bl.Call.UpdateCompleteAssignment(CurrentVolunteer.Id, CurrentCall.Id);
            MessageBox.Show("Treatment completed successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (BO.BlTemporaryNotAvailableException ex)
        {
            MessageBox.Show($"{ex.Message}\nPlease stop the Simulator and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        catch (Exception ex)
        {
            MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
        }
    }

    /// <summary>
    /// Event handler for when the window is loaded.
    /// </summary>
    private void VolunteerUserWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
        {
            if(CurrentCall!=null)
            MapWebView.NavigationCompleted += async (s, args) =>
            {
                CenterMapOnVolunteer();
            };
            s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, VolunteerObserver);
            VolunteerObserver();
        }
    }
    private async void CenterMapOnVolunteer()
    {
        if (CurrentCall is not null)
        {
            await MapWebView.ExecuteScriptAsync($"initialize({CurrentVolunteer.Latitude}, " +
                $"{CurrentVolunteer.Longitude}, {CurrentCall.Latitude}, " +
                $"{CurrentCall.Longitude}, '{CurrentVolunteer.TypeDistance}');");
        }
        else
        {
            await MapWebView.ExecuteScriptAsync($"initialize({CurrentVolunteer.Latitude}, " +
                $"{CurrentVolunteer.Longitude}, {CurrentVolunteer.Latitude}, " +
                $"{CurrentVolunteer.Longitude}, '{CurrentVolunteer.TypeDistance}');");
        }
    }

    /// <summary>
    /// Event handler for when the window is closed.
    /// </summary>
    private void VolunteerUserWindow_Closed(object? sender, EventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, VolunteerObserver);
    }

    /// <summary>
    /// Observer method to update volunteer and call details.
    /// </summary>
    private volatile DispatcherOperation? _observerOperation = null; //stage 7 

    private void VolunteerObserver()
    {
        try
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {

                    int id = CurrentVolunteer?.Id ?? 0;
                    int idC = CurrentCall?.Id ?? 0;
                    CurrentVolunteer = null;
                    CurrentCall = null;
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                    CurrentCall = CurrentVolunteer?.CurrentCall != null ? s_bl.Call.GetCallDetails(CurrentVolunteer.CurrentCall.CallId) : null;
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
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

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerUserWindow1.xaml
/// </summary>
public partial class VolunteerUserWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

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
    public VolunteerUserWindow(int id)
    {
        InitializeComponent();
        Loaded += VolunteerUserWindow_Loaded;
        Closed += VolunteerUserWindow_Closed;
        CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id)! : new BO.Volunteer();
        CurrentCall = CurrentVolunteer.CurrentCall != null ? s_bl.Call.GetCallDetails(CurrentVolunteer.CurrentCall.CallId)! : new BO.Call();
    }
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
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void btnCallHistory_Click(object sender, RoutedEventArgs e)
    {
       new VolunteerCallHistoryWindow(CurrentVolunteer.Id).Show();
    }

    private void btnSelectCall_Click(object sender, RoutedEventArgs e)
    {
        new SelectCallToTreatWindow(CurrentVolunteer.Id).Show();
    }

    private void btnCancelCall_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentCall == null)
        {
            MessageBox.Show("Call details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        try
        {
            s_bl.Call.UpdateCancelTreatment(CurrentVolunteer.Id,CurrentCall.Id);
            MessageBox.Show("Treatment cancellation successfully canceled", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
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
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void VolunteerUserWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
        { 
            s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, VolunteerObserver);
        VolunteerObserver(); 
    }
    }

    private void VolunteerUserWindow_Closed(object? sender, EventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, VolunteerObserver);
    }

    private void VolunteerObserver()
    { 
        int id = CurrentVolunteer?.Id??0;
        int idC = CurrentCall?.Id??0;
        CurrentVolunteer = null;
        CurrentCall = null;
        CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        CurrentCall = CurrentVolunteer?.CurrentCall != null ? s_bl.Call.GetCallDetails(CurrentVolunteer.CurrentCall.CallId)! : null;
    }
}

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

namespace PL.Call;

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
    public SelectCallToTreatWindow(int idV)
    {id= idV;
        InitializeComponent();
        Loaded += CallListWindow_Loaded;
        Closed += CallListWindow_Closed;
        CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id)! : new BO.Volunteer();
    }
    private void CallListWindow_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(callListObserver);
    }
    private void queryCallList()
    {
        OpenCallList = (FilterByType == BO.MyCallType.None) ?
       s_bl?.Call.SortOpenedCalls(id, null, null)! :
       s_bl?.Call.SortOpenedCalls(id, FilterByType, null)!;

    }
    private void callListObserver()
    {
        queryCallList();
    }
    private void CallListWindow_Closed(object? sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(callListObserver);
    }

    private void cmbFiltedrChanges_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        queryCallList();

    }
    private void SelectCall_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Call.SelectCallToTreat(id, SelectedOpenCall!.Id);
        MessageBox.Show("A call has been selected for treatment", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        Close();
        new VolunteerUserWindow(id).Show();
    }
    private void DataGrid_MouseLeftButtonUp(object sender, RoutedEventArgs e)
    {
List<string> addresses = OpenCallList.Select(call => call.Address).ToList();
        new CallDescription(addresses, CurrentVolunteer.Address,id).Show();
    }

    private void UpdateAddress_Click(object sender, RoutedEventArgs e)
    {

        s_bl.Volunteer.UpdateVolunteer(id,CurrentVolunteer);
        MessageBox.Show("The address has been updated successfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            queryCallList();
    }
}

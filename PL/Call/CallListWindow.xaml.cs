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
/// Interaction logic for CallListWindow.xaml
/// </summary>
public partial class CallListWindow : Window
{
    public BO.CallInList? SelectedCall { get; set; }

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.MyCallStatus SortInCallInList { get; set; } = BO.MyCallStatus.None;

    public CallListWindow()
    {
        InitializeComponent();
        Loaded += CallListWindow_Loaded;
        Closed += CallListWindow_Closed;
    }
    public IEnumerable<BO.CallInList> CallList
    {
        get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CallList.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));
   
    private void cmbSelectChanges_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        queryCallList();
    }
    private void queryCallList()
    {
        CallList = (SortInCallInList == BO.MyCallStatus.None) ?
          s_bl?.Call.GetFilterCallList(BO.MyCallStatus.None)! :
          s_bl?.Call.GetFilterCallList(SortInCallInList)!;
    }
    private void callListObserver()
    {
        queryCallList();
    }
    private void CallListWindow_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(callListObserver);
    }

    private void CallListWindow_Closed(object? sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(callListObserver);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        new CallWindow().Show();
    }
    private void UpdateCallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall != null)
            new CallWindow(SelectedCall.CallId).Show();
    }
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
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}


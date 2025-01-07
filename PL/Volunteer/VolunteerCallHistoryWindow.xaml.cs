using BO;
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
/// Interaction logic for VolunteerCallHistoryWindow.xaml
/// </summary>
public partial class VolunteerCallHistoryWindow : Window
{
    public BO.ClosedCallInList? SelectedClosedCall { get; set; }

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.MyCallType FilterByType { get; set; } = BO.MyCallType.None;
    public BO.CloseCall SortByClosedCall { get; set; } = BO.CloseCall.None;

    public IEnumerable<BO.ClosedCallInList> CloseCallList
    {
        get { return (IEnumerable<BO.ClosedCallInList>)GetValue(CloseCallListProperty); }
        set { SetValue(CloseCallListProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CallList.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CloseCallListProperty =
        DependencyProperty.Register("CloseCallList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(VolunteerCallHistoryWindow), new PropertyMetadata(null));

    private int id;
    public VolunteerCallHistoryWindow(int idV)
    {
        id = idV;
        InitializeComponent();
        Loaded += CallListWindow_Loaded;
        Closed += CallListWindow_Closed;
    }
    private void CallListWindow_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(callListObserver);
    }
    private void queryCallList()
    {
        // OpenCallList=s_bl.Call.GetFilterCallList(BO.MyCallStatus.Open)
        // OpenCallList = (FilterByType == BO.MyCallType.None) ?
        //s_bl?.Call.SortOpenedCalls(id, BO.MyCallType.None, null)! :
        //s_bl?.Call.SortOpenedCalls(id, FilterByType, null)!;

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
        CloseCallList = (FilterByType == BO.MyCallType.None) ?
 s_bl?.Call.SortClosedCalls(id, BO.MyCallType.None, null)! :
 s_bl?.Call.SortClosedCalls(id, FilterByType, null)!;
    }

    private void cmbSortChanges_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CloseCallList = (SortByClosedCall == BO.CloseCall.None) ?
s_bl?.Call.SortClosedCalls(id, null, BO.CloseCall.None)! :
s_bl?.Call.SortClosedCalls(id, null, SortByClosedCall)!;
    }
}

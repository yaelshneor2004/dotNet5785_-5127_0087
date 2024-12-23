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
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.MySortInCallInList SortInCallInList { get; set; } = BO.MySortInCallInList.All;

    public CallListWindow()
    {
        InitializeComponent();
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
        CallList = (SortInCallInList == BO.MySortInCallInList.All) ? s_bl?.Call.GetCallList(null, null, null)! : s_bl?.Call.GetCallList(null, null, SortInCallInList)!;
    }
}


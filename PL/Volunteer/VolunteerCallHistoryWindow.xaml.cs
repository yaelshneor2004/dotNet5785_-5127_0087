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
using System.Windows.Threading;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerCallHistoryWindow.xaml
    /// </summary>
    public partial class VolunteerCallHistoryWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.MyCallType FilterByType { get; set; } = BO.MyCallType.None;

        /// <summary>
        /// Gets or sets the list of closed calls.
        /// </summary>
        public IEnumerable<BO.ClosedCallInList>CloseCallList 
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(CloseCallListProperty); }
            set { SetValue(CloseCallListProperty, value); }
        }

        public static readonly DependencyProperty CloseCallListProperty =
            DependencyProperty.Register("CloseCallList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(VolunteerCallHistoryWindow), new PropertyMetadata(null));

        private int id;

        /// <summary>
        /// Constructor for VolunteerCallHistoryWindow. Initializes components.
        /// </summary>
        /// <param name="idV">ID of the volunteer.</param>
        public VolunteerCallHistoryWindow(int idV)
        {
            id = idV;
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for window loaded event. Adds observer for the call list.
        /// </summary>
        private void CallListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(callListObserver);
        }

        /// <summary>
        /// Queries the call list based on the filter by type.
        /// </summary>
        private void queryCallList()
        {
            CloseCallList = (FilterByType == BO.MyCallType.None) ?
                s_bl?.Call.SortClosedCalls(id, null, null)! :
                s_bl?.Call.SortClosedCalls(id, FilterByType, null)!;
        }

        /// <summary>
        /// Observer method to update the call list.
        /// </summary>
        private volatile DispatcherOperation? _observerOperation = null; 

        private void callListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                    queryCallList()
                );
        }

        /// <summary>
        /// Event handler for window closed event. Removes observer for the call list.
        /// </summary>
        private void CallListWindow_Closed(object? sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(callListObserver);
        }

        /// <summary>
        /// Event handler for filter changes in the combo box. Re-queries the call list.
        /// </summary>
        private void cmbFiltedrChanges_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCallList();
        }
    }
}

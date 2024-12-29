using BO;
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

namespace PL.Volunteer
{

    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.VolunteerInList ? SelectedVolunteer { get; set; }
        public BO.MyCallType SortInVolunteerInList { get; set; } = BO.MyCallType.None;

        public VolunteerListWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }

        public IEnumerable<BO.VolunteerInList> VolunteerInList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerInListProperty); }
            set { SetValue(VolunteerInListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerInListProperty =
            DependencyProperty.Register("VolunteerInList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        /// <summary>
        /// Handles the selection change event for the volunteer list combo box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void cmbVolunteerInList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryVolunteerList();
        }

        /// <summary>
        /// Queries the volunteer list based on the selected sort type.
        /// </summary>
        private void queryVolunteerList()
        {
            VolunteerInList = (SortInVolunteerInList == BO.MyCallType.None) ?
                s_bl?.Volunteer.GetFilterVolunteerList(BO.MyCallType.None)! :
                s_bl?.Volunteer.GetFilterVolunteerList(SortInVolunteerInList)!;
        }

        /// <summary>
        /// Observes changes in the volunteer list and updates it accordingly.
        /// </summary>
        private void VolunteerListObserver()
        {
            queryVolunteerList();
        }

        /// <summary>
        /// Handles the window loaded event and adds the volunteer list observer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
        }

        /// <summary>
        /// Handles the window closed event and removes the volunteer list observer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Window_Closed(object? sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
        }

        /// <summary>
        /// Handles the mouse double-click event to update the volunteer list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void UpdateVolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow(SelectedVolunteer.Id).Show();
        }

        /// <summary>
        /// Handles the click event to add a new volunteer to the list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void AddVolunteerList_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }
        /// <summary>
        /// Handles the click event to delete a selected volunteer from the list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void DeleteVolunteer_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedVolunteer == null)
            {
                MessageBox.Show("No volunteer selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this volunteer?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.DeleteVolunteer(SelectedVolunteer.Id);
                    MessageBox.Show("Volunteer deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}

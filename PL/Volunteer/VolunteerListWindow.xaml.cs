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
        public BO.MyCallType    ? SelectedVolunteer { get; set; }
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

        public BO.MyCallType SortInVolunteerInList { get; set; } = BO.MyCallType.None;

        private void cmbVolunteerInList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryVolunteerList();
        }

        private void queryVolunteerList()
        {
            VolunteerInList = (SortInVolunteerInList == BO.MyCallType.None) ?
                s_bl?.Volunteer.GetFilterVolunteerList(BO.MyCallType.None)! :
                s_bl?.Volunteer.GetFilterVolunteerList(SortInVolunteerInList)!;
        }

        private void VolunteerListObserver()
        {
            queryVolunteerList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
        }

        private void UpdateVolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow(SelectedVolunteer.Id).Show();
        }

        private void AddVolunteerList_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }
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

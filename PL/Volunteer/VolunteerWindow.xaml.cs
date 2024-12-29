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
        public BO.MyCallStatusByVolunteer CallStatus{ get; set; } = BO.MyCallStatusByVolunteer.None;
                private int id = 0;
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(string.Empty));

        public VolunteerWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            InitializeComponent();
            Loaded += VolunteerWindow_Loaded;
            Closed += VolunteerWindow_Closed;
            if (id == 0)
            {
                CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id)! : new BO.Volunteer()
                {
                    Id = 0,
                    FullName = "",
                    Phone = "",
                    Email = "",
                    Password = null,
                    Address = null,
                    Latitude = null,
                    Longitude = null,
                    Role = BO.MyRole.Volunteer,
                    IsActive = true,
                    MaxDistance = null,
                    TypeDistance = BO.MyTypeDistance.Aerial,
                    TotalCallsCancelled = 0,
                    TotalCallsExpired = 0,
                    TotalCallsHandled = 0,
                    CurrentCall = null,
                };

            }
            else
            {
                CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id)! : new BO.Volunteer()
                {
                    Id = CurrentVolunteer.Id,
                    FullName = CurrentVolunteer.FullName,
                    Phone = CurrentVolunteer.Phone,
                    Email = CurrentVolunteer.Email,
                    Password = CurrentVolunteer.Password,
                    Address = CurrentVolunteer.Address,
                    Latitude = CurrentVolunteer.Latitude,
                    Longitude = CurrentVolunteer.Longitude,
                    Role = CurrentVolunteer.Role,
                    IsActive = CurrentVolunteer.IsActive,
                    MaxDistance = CurrentVolunteer.MaxDistance,
                    TypeDistance = CurrentVolunteer.TypeDistance,
                    TotalCallsHandled = CurrentVolunteer.TotalCallsHandled,
                    TotalCallsCancelled = CurrentVolunteer.TotalCallsCancelled,
                    TotalCallsExpired = CurrentVolunteer.TotalCallsExpired,
                    CurrentCall = CurrentVolunteer.CurrentCall,
                };
            }


            //try
            //{
            //    CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id) : new BO.Volunteer(0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, null, BO.MyRole.Volunteer, true, null, BO.MyTypeDistance.Aerial, 0, 0, 0, null);
            //    if (CurrentVolunteer!.Id != 0)
            //    {
            //        s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, VolunteerObserver);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}

            //DataContext = CurrentVolunteer;
        }

        private void VolunteerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.Id != 0 )
                s_bl.Volunteer.AddObserver( CurrentVolunteer.Id,VolunteerObserver);
        }

        private void VolunteerWindow_Closed(object? sender, EventArgs e)
        {
            if (CurrentVolunteer!.Id != 0 )
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, VolunteerObserver);
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer == null)
            {
                MessageBox.Show("Volunteer details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ButtonText == "Add")
            {
                try
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                    MessageBox.Show("Volunteer added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    s_bl.Volunteer.UpdateVolunteer(id, CurrentVolunteer);
                    MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void VolunteerObserver()
        {
            int id = CurrentVolunteer!.Id;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        }
       
    }
   
}

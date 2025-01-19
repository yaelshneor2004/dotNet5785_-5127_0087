using PL.Volunteer;
using System;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private static bool isManagerLoggedIn = false;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public LoginWindow()
        {
            InitializeComponent();
        }

        public string TextBoxId
        {
            get { return (string)GetValue(TextBoxIdProperty); }
            set { SetValue(TextBoxIdProperty, value); }
        }

        public static readonly DependencyProperty TextBoxIdProperty =
            DependencyProperty.Register("TextBoxId", typeof(string), typeof(LoginWindow), new PropertyMetadata(string.Empty));

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            int id;
            if (string.IsNullOrWhiteSpace(TextBoxId) || !int.TryParse(TextBoxId, out id))
            {
                MessageBox.Show("Please enter a valid ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BO.Volunteer v = s_bl.Volunteer.GetVolunteerDetails(id);

            if (v.Role == BO.MyRole.Manager)
            {
                if (isManagerLoggedIn)
                {
                    MessageBox.Show("Administrator is already logged in, please wait until the connection is complete");
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to open the main screen?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        var main = new MainWindow(v.Id);
                        main.Closed += (s, e) => isManagerLoggedIn = false;
                        main.Show();
                        isManagerLoggedIn = true;
                    }
                    else
                    {
                        new VolunteerUserWindow(v.Id).Show();
                    }
                }
            }
            else // this is volunteer
            {
                new VolunteerUserWindow(id).Show();
            }
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                PasswordBox.Visibility = Visibility.Collapsed;
                VisiblePassword.Visibility = Visibility.Visible;
                VisiblePassword.Text = PasswordBox.Password;
            }
            else
            {
                PasswordBox.Visibility = Visibility.Visible;
                VisiblePassword.Visibility = Visibility.Collapsed;
                PasswordBox.Password = VisiblePassword.Text;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            VisiblePassword.Text = PasswordBox.Password;
        }

        private void VolunteerUserWindow_Click(object sender, RoutedEventArgs e)
        {
            int id;
            if (!int.TryParse(TextBoxId, out id) || id == 0)
            {
                MessageBox.Show("Please enter a valid ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BO.Volunteer v = s_bl.Volunteer.GetVolunteerDetails(id);
            new VolunteerUserWindow(id).Show();
        }

        private void CloseAllWindows()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                    window.Close();
            }
            isManagerLoggedIn = false;
        }
    }
}

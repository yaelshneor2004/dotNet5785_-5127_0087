using PL.Volunteer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        private static PasswordBox? FindPasswordBox(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is PasswordBox passwordBox)
                    return passwordBox;
                var result = FindPasswordBox(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private static TextBox? FindPasswordTextBox(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TextBox textBox && textBox.Visibility == Visibility.Collapsed)
                    return textBox;
                var result = FindPasswordTextBox(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            var passwordBox = FindPasswordBox(this);
            var passwordTextBox = FindPasswordTextBox(this);

            if (passwordBox == null || passwordTextBox == null)
                return;

            if (passwordBox.Visibility == Visibility.Visible)
            {
                passwordBox.Visibility = Visibility.Collapsed;
                passwordTextBox.Visibility = Visibility.Visible;
                passwordTextBox.Text = passwordBox.Password;
            }
            else
            {
                passwordBox.Visibility = Visibility.Visible;
                passwordTextBox.Visibility = Visibility.Collapsed;
                passwordBox.Password = passwordTextBox.Text;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = FindPasswordBox(this);
            var passwordTextBox = FindPasswordTextBox(this);

            if (passwordBox != null && passwordTextBox != null)
                passwordTextBox.Text = passwordBox.Password;
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

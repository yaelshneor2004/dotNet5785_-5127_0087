using BO;
using PL.Volunteer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PL
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
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
        private static bool isManagerLoggedIn = false;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public LoginWindow()
        {
            InitializeComponent();
        }

        public string userNameText { get; set; }
        public string ButtonImageSource
        {
            get { return (string)GetValue(ButtonImageSourceProperty); }
            set { SetValue(ButtonImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ButtonImageSourceProperty =
            DependencyProperty.Register("ButtonImageSource", typeof(string), typeof(LoginWindow), new PropertyMetadata("/Resources/closeEye.jpg"));

        public Visibility passwordVisibility
        {
            get { return (Visibility)GetValue(passwordVisibilityProperty); }
            set { SetValue(passwordVisibilityProperty, value); }
        }

        public static readonly DependencyProperty passwordVisibilityProperty =
            DependencyProperty.Register("passwordVisibility", typeof(Visibility), typeof(LoginWindow), new PropertyMetadata(Visibility.Visible));

        public Visibility TextVisibility
        {
            get { return (Visibility)GetValue(TextVisibilityProperty); }
            set { SetValue(TextVisibilityProperty, value); }
        }

        public static readonly DependencyProperty TextVisibilityProperty =
            DependencyProperty.Register("TextVisibility", typeof(Visibility), typeof(LoginWindow), new PropertyMetadata(Visibility.Collapsed));

        public string passwordText
        {
            get { return (string)GetValue(passwordTextProperty); }
            set { SetValue(passwordTextProperty, value); }
        }

        public static readonly DependencyProperty passwordTextProperty =
            DependencyProperty.Register("passwordText", typeof(string), typeof(LoginWindow), new PropertyMetadata(string.Empty));

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userDetails = s_bl.Volunteer.Login(userNameText, FindPasswordBox(this)!.Password);
                // Show message to ask if the user wants to enter as a manager or volunteer
                MessageBoxResult result = MessageBox.Show("Do you want to open the main screen?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) // User chooses to enter as manager
                {
                    if (userDetails.Item1 == BO.MyRole.Manager)
                    {
                        if (isManagerLoggedIn)
                        {
                            MessageBox.Show("Administrator is already logged in, please wait until the connection is complete");
                        }
                        else
                        {
                            var main = new MainWindow(userDetails.Item2);
                            main.Closed += (s, e) => isManagerLoggedIn = false;
                            main.Show();
                            isManagerLoggedIn = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You do not have manager privileges.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else // User chooses to enter as volunteer
                {
                    new VolunteerUserWindow(userDetails.Item2).Show();

                }
                //new MainWindow(326615127).Show();
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
                    catch (BO.BlUnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
            }
        }


        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            if (passwordVisibility == Visibility.Visible)
            {
                TextVisibility = Visibility.Visible;
                passwordVisibility = Visibility.Collapsed;
                passwordText = FindPasswordBox(this)!.Password;
                ButtonImageSource = "/Resources/closeEye.jpg";
            }
            else
            {
                passwordText = FindPasswordBox(this)!.Password;
                passwordVisibility = Visibility.Visible;
                TextVisibility = Visibility.Collapsed;
                ButtonImageSource = "/Resources/openEye.jpg";
            }
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
    }
}

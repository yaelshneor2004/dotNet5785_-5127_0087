using PL.Volunteer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PL;

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
    public int userNameText {  get;  set; }
    public Visibility passwordVisibility
    {
        get { return (Visibility)GetValue(passwordVisibilityProperty); }
        set { SetValue(passwordVisibilityProperty, value); }
    }

    public static readonly DependencyProperty passwordVisibilityProperty =
        DependencyProperty.Register("passwordVisibility", typeof(Visibility), typeof(LoginWindow), new PropertyMetadata(Visibility.Collapsed));
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
            var userDetails = s_bl.Volunteer.GetVolunteerDetails(userNameText);
            int id = userDetails.Id;
            if (userDetails.Password != FindPasswordBox(this)!.Password)
            {
                MessageBox.Show("The password you entered does not match our records. Please try again.", "Password Mismatch", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


            if (userDetails.Role == BO.MyRole.Manager)
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
                        var main = new MainWindow(id);
                        main.Closed += (s, e) => isManagerLoggedIn = false;
                        main.Show();
                        isManagerLoggedIn = true;
                    }

                    else
                        new VolunteerUserWindow(id).Show();
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
        catch (BO.BlDoesNotExistException ex)
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
            passwordText=FindPasswordBox(this)!.Password;

        }
        else
        {
            passwordText = FindPasswordBox(this)!.Password;
            passwordVisibility = Visibility.Visible;
            TextVisibility = Visibility.Collapsed;

        }
    }
    //private void Window_closing()
    //{
    //    foreach (Window window in Application.Current.Windows)
    //    {
    //        if (window != this)
    //            window.Close();
    //    }
    //    isManagerLoggedIn = false;
    //}

}

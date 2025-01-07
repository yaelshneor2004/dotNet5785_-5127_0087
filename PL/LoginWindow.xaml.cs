using PL.Volunteer;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
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

        public string TextBoxPassword
        {
            get { return (string)GetValue(TextBoxPasswordProperty); }
            set { SetValue(TextBoxPasswordProperty, value); }
        }
        public static readonly DependencyProperty TextBoxPasswordProperty =
            DependencyProperty.Register("TextBoxPassword", typeof(string), typeof(LoginWindow), new PropertyMetadata(string.Empty));
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            int id =Convert.ToInt32(TextBoxId);
            VolunteerUserWindow volunteerWindow = new VolunteerUserWindow(id);
            volunteerWindow.Show();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}

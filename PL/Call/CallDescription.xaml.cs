using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using System.Text.Json;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallDescription.xaml
    /// </summary>
    public partial class CallDescription : Window
    {
        private const string GoogleMapsApiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4";

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        // public BO.Call? SelectedCall { get; set; }
        public string Description{get;set;}
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(CallDescription), new PropertyMetadata(null));

        /// <summary>
        /// Constructor for CallDescription window. Initializes components and loads call details.
        /// </summary>
        /// <param name="callAddresses">List of call addresses.</param>
        /// <param name="volunteerAddress">Address of the volunteer.</param>
        /// <param name="id">ID of the call.</param>
        public CallDescription(int id,string description)
        {
            try
            {
                Description = description;
                InitializeComponent();

                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);//טיפול בחריגה
                MapSource = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "mapp.html");

                //LoadMapAsync(callAddresses, volunteerAddress);
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

        /// <summary>
        /// Event handler for window loaded event. Adds observer for the current call.
        /// </summary>
        public BO.Call? SelectedCall
        {
            get { return (BO.Call?)GetValue(SelectedCallProperty); }
            set { SetValue(SelectedCallProperty, value); }
        }
        public static readonly DependencyProperty SelectedCallProperty =
        DependencyProperty.Register("SelectedCall", typeof(BO.Call), typeof(CallDescription), new PropertyMetadata(null));
        public string MapSource
        {
            get { return (string)GetValue(MapSourceProperty); }
            set { SetValue(MapSourceProperty, value); }
        }
        public static readonly DependencyProperty MapSourceProperty =
        DependencyProperty.Register("MapSource", typeof(string), typeof(CallDescription), new PropertyMetadata(null));

        private void VolunteerListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //s_bl.Call.AddObserver(OpenCallsListObserver);
            //s_bl.Volunteer.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);
            MapWebView.NavigationCompleted += async (s, args) =>
            {
                CenterMapOnVolunteer();
            };
        }
        private async void CenterMapOnVolunteer()
        {
            if (SelectedCall is not null)
            {
                await MapWebView.ExecuteScriptAsync($"initialize({CurrentVolunteer.Latitude}, " +
                $"{CurrentVolunteer.Longitude}, {SelectedCall.Latitude}, " +
                $"{SelectedCall.Longitude});");
            }
            else
            {
                await MapWebView.ExecuteScriptAsync($"initialize({CurrentVolunteer.Latitude}, " +
                $"{CurrentVolunteer.Longitude}, {CurrentVolunteer.Latitude}, " +
                $"{CurrentVolunteer.Longitude});");
            }
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null)
                return;

            if (dataGrid.SelectedItem is BO.OpenCallInList openCall)
            {
                SelectedCall = s_bl.Call.GetCallDetails(openCall.Id);
                CenterMapOnVolunteer();
            }
        }

    }

}

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
        public BO.Call? SelectedCall { get; set; }

        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallDescription), new PropertyMetadata(null));

        /// <summary>
        /// Constructor for CallDescription window. Initializes components and loads call details.
        /// </summary>
        /// <param name="callAddresses">List of call addresses.</param>
        /// <param name="volunteerAddress">Address of the volunteer.</param>
        /// <param name="id">ID of the call.</param>
        public CallDescription(List<string> callAddresses, string volunteerAddress, int id)
        {
            try
            {
                InitializeComponent();
                Loaded += CallWindow_Loaded;
                Closed += CallWindow_Closed;
                CurrentCall = s_bl.Call.GetCallDetails(id);
                LoadMapAsync(callAddresses, volunteerAddress);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unknown error occurred: {ex.Message}.", "Unknown Error");
            }
        }

        /// <summary>
        /// Event handler for window loaded event. Adds observer for the current call.
        /// </summary>
        private void CallWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCall!.Id != 0)
                s_bl.Call.AddObserver(CurrentCall.Id, CallObserver);
        }

        /// <summary>
        /// Event handler for window closed event. Removes observer for the current call.
        /// </summary>
        private void CallWindow_Closed(object? sender, EventArgs e)
        {
            if (CurrentCall!.Id != 0)
                s_bl.Call.RemoveObserver(CurrentCall.Id, CallObserver);
        }

        /// <summary>
        /// Observer method to update the current call details.
        /// </summary>
        private void CallObserver()
        {
            try
            {
                int id = CurrentCall!.Id;
                CurrentCall = null;
                CurrentCall = s_bl.Call.GetCallDetails(id);
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
        /// Asynchronously loads the map with call addresses and volunteer address.
        /// </summary>
        private async void LoadMapAsync(List<string> callAddresses, string volunteerAddress)
        {
            string html = await GenerateMapHtmlAsync(callAddresses, volunteerAddress);
            webBrowser.NavigateToString(html); // Use the WebBrowser control from XAML
        }

        /// <summary>
        /// Generates HTML for the map with markers using Google Maps API.
        /// </summary>
        private async Task<string> GenerateMapHtmlAsync(List<string> callAddresses, string volunteerAddress)
        {
            string markersHtml = "";
            foreach (var address in callAddresses)
            {
                var location = await GetLatLngFromAddressAsync(address);
                markersHtml += $"<gmp-advanced-marker position='{location}' title='Call Location'></gmp-advanced-marker>";
            }

            var volunteerLocation = await GetLatLngFromAddressAsync(volunteerAddress);

            string html = $@"
            <html>
              <head>
                <title>Add a Map with Markers using HTML</title>
                <link rel='stylesheet' type='text/css' href='./style.css' />
                <script type='module' src='./index.js'></script>
              </head>
              <body>
                <gmp-map
                  center='{volunteerLocation}'
                  zoom='10'
                  map-id='DEMO_MAP_ID'
                  style='height: 400px'
                >
                  {markersHtml}
                </gmp-map>

                <script
                  src='https://maps.googleapis.com/maps/api/js?key={GoogleMapsApiKey}&libraries=maps,marker&v=beta'
                  defer
                ></script>
              </body>
            </html>";
            return html;
        }

        /// <summary>
        /// Gets latitude and longitude from an address asynchronously.
        /// </summary>
        private async Task<string> GetLatLngFromAddressAsync(string address)
        {
            var (latitude, longitude) = await Task.Run(() => GetCoordinates(address));
            return $"{latitude},{longitude}";
        }

        /// <summary>
        /// Gets latitude and longitude coordinates from an address.
        /// </summary>
        public static (double Latitude, double Longitude) GetCoordinates(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address cannot be null or empty.");
            }

            var apiKey = GoogleMapsApiKey;
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";

            using (var client = new HttpClient())
            {
                var response = client.GetStringAsync(url).Result;

                // Analyze the response in JSON format
                var jsonResponse = JsonDocument.Parse(response);

                if (jsonResponse.RootElement.TryGetProperty("results", out JsonElement results) && results.GetArrayLength() > 0)
                {
                    // The reference to the first result
                    var firstResult = results[0];

                    // Extracting the coordinates
                    var location = firstResult.GetProperty("geometry").GetProperty("location");
                    var latitude = location.GetProperty("lat").GetDouble();
                    var longitude = location.GetProperty("lng").GetDouble();

                    return (latitude, longitude);
                }
                else
                {
                    throw new Exception("No results found for the given address.");
                }
            }
        }
    }
}

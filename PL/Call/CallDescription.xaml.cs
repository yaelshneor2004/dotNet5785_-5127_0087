using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Net.Http;
using System.Text.Json;

namespace PL.Call;

/// <summary>
/// Interaction logic for CallDescription.xaml
/// </summary>
public partial class CallDescription : Window
{
    private const string GoogleMapsApiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4";



    public string description
    {
        get { return (string)GetValue(descriptionProperty); }
        set { SetValue(descriptionProperty, value); }
    }

    // Using a DependencyProperty as the backing store for description.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty descriptionProperty =
        DependencyProperty.Register("description", typeof(string), typeof(CallDescription), new PropertyMetadata(null));


    public CallDescription(List<string> callAddresses, string volunteerAddress,string description)
    {
        InitializeComponent();
        //LoadMapAsync(callAddresses, volunteerAddress);
    }

    //private async void LoadMapAsync(List<string> callAddresses, string volunteerAddress)
    //{
    //    string html = await GenerateMapHtmlAsync(callAddresses, volunteerAddress);
    //    Console.WriteLine(html);
    //    webBrowser.NavigateToString(html);
    //}

    //private async Task<string> GenerateMapHtmlAsync(List<string> callAddresses, string volunteerAddress)
    //{
    //    string markers = "";
    //    string lines = "";

    //    var volunteerLocation = await GetLatLngFromAddressAsync(volunteerAddress);
    //    markers += $"new google.maps.Marker({{ position: {volunteerLocation}, map: map, title: 'Volunteer' }});";

    //    foreach (var address in callAddresses)
    //    {
    //        var callLocation = await GetLatLngFromAddressAsync(address);
    //        markers += $"new google.maps.Marker({{ position: {callLocation}, map: map, title: 'Call' }});";
    //        lines += $"new google.maps.Polyline({{ path: [{volunteerLocation}, {callLocation}], geodesic: true, strokeColor: '#FF0000', strokeOpacity: 1.0, strokeWeight: 2 }}).setMap(map);";
    //    }

    //    string html = "<html><head><meta charset='UTF-8'><title>Map</title>"; html += $"<script src='https://maps.googleapis.com/maps/api/js?key={GoogleMapsApiKey}'></script>"; html += "<script>function initialize() {"; html += $"var mapOptions = {{ zoom: 12, center: {volunteerLocation} }};"; html += "var map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);"; html += $"{markers} {lines} "; html += "} google.maps.event.addDomListener(window, 'load', initialize);</script>"; html += "</head><body><div id='map-canvas' style='width:100%; height:100%;'></div></body></html>"; return html;
    //}

    //private async Task<string> GetLatLngFromAddressAsync(string address)
    //{
    //    var (latitude, longitude) = await Task.Run(() => GetCoordinates(address));
    //    return $"{{ lat: {latitude}, lng: {longitude} }}";
    //}
    //public static (double Latitude, double Longitude) GetCoordinates(string address)
    //{
    //    if (string.IsNullOrWhiteSpace(address))
    //    {
    //        throw new ArgumentException("Address cannot be null or empty.");
    //    }


    //    var apiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4";
    //    var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";

    //    using (var client = new HttpClient())
    //    {
    //        var response = client.GetStringAsync(url).Result;

    //        // Analyze the response in JSON format\
    //        var jsonResponse = JsonDocument.Parse(response);

    //        if (jsonResponse.RootElement.TryGetProperty("results", out JsonElement results) && results.GetArrayLength() > 0)
    //        {
    //            // The reference to the first result
    //            var firstResult = results[0];

    //            // Extracting the coordinates
    //            var location = firstResult.GetProperty("geometry").GetProperty("location");
    //            var latitude = location.GetProperty("lat").GetDouble();
    //            var longitude = location.GetProperty("lng").GetDouble();

    //            return (latitude, longitude);
    //        }
    //        else
    //        {
    //            throw new Exception("No results found for the given address.");
    //        }
    //    }
    //}



}

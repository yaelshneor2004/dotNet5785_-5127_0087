
using BO;
using DalApi;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System;
using System.Net.Http;
using System.Text.Json;
namespace Helpers;
internal static class Tools
{
    private const string ApiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4";
    private const string GoogleGeocodingApiUrl = "https://geocode.maps.co/search?q={0}&api_key=675ef7e408d33282453687qrh963303";
    private const string GoogleMapsApiUrl = "https://geocode.maps.co/reverse?lat=&lon=&api_key=675ef7e408d33282453687qrh963303";
private static IDal s_dal = Factory.Get;
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return string.Empty;

        StringBuilder sb = new StringBuilder();
        Type type = t.GetType();
        PropertyInfo[] properties = type.GetProperties();

        foreach (PropertyInfo property in properties)
        {
            object value = property.GetValue(t, null);

            if (value is System.Collections.IEnumerable && !(value is string))
            {
                sb.AppendLine($"{property.Name} = [");
                foreach (var item in (System.Collections.IEnumerable)value)
                {
                    sb.AppendLine($"  {item},");
                }
                sb.AppendLine("]");
            }
            else
            {
                sb.AppendLine($"{property.Name} = {value}");
            }
        }

        return sb.ToString();
    }
    public static double GlobalDistance(double? lat1, double? lon1, double lat2, double lon2, DO.MyTypeDistance myTypeDistance)
    {
        if (lat1 == null || lon1 == null)
            throw new BlInvalidOperationException("Latitude and Longitude cannot be null.");
        return myTypeDistance switch
        {
            DO.MyTypeDistance.Aerial => CalculateAerialDistance(lat1.Value, lon1.Value, lat2, lon2),
            DO.MyTypeDistance.Walking => CalculateWalkingDistance(lat1.Value, lon1.Value, lat2, lon2),
            DO.MyTypeDistance.Traveling => CalculateDrivingDistance(lat1.Value, lon1.Value, lat2, lon2),
            _ => throw new BlInvalidOperationException("Invalid distance type")
        };
    }
    private static double CalculateAerialDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var lat = ToRadians(lat2 - lat1);
        var lon = ToRadians(lon2 - lon1);
        var a = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(lon / 2) * Math.Sin(lon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
    private static double ToRadians(double angle)
    {
        return angle * (Math.PI / 180);
    }
    // Calculate the distance in kilometers
    public static double CalculateDrivingDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var apiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4"; 
        var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={lat1},{lon1}&destinations={lat2},{lon2}&mode=driving&key={apiKey}";

        using HttpClient client = new HttpClient();
        var response = client.GetAsync(url).Result;

        if (response.IsSuccessStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            using var jsonDocument = JsonDocument.Parse(content);

            if (jsonDocument.RootElement.TryGetProperty("rows", out JsonElement rowsElement) && rowsElement.GetArrayLength() > 0)
            {
                var firstRow = rowsElement[0];
                if (firstRow.TryGetProperty("elements", out JsonElement elementsElement) && elementsElement.GetArrayLength() > 0)
                {
                    var firstElement = elementsElement[0];
                    if (firstElement.TryGetProperty("distance", out JsonElement distanceElement))
                    {
                        var distanceInMeters = distanceElement.GetProperty("value").GetDouble();
                        return distanceInMeters / 1000.0; // distance in kilometers
                    }
                }
            }
            throw new Exception("Failed to calculate driving distance.");
        }
        else
        {
            throw new Exception($"Failed to call API: {response.ReasonPhrase}");
        }
    }


    // Calculate walking distance between two coordinate points
    public static double CalculateWalkingDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var apiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4"; 
        var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={lat1},{lon1}&destinations={lat2},{lon2}&mode=walking&key={apiKey}";

        using HttpClient client = new HttpClient();
        var response = client.GetAsync(url).Result;

        if (response.IsSuccessStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            using var jsonDocument = JsonDocument.Parse(content);

            if (jsonDocument.RootElement.TryGetProperty("rows", out JsonElement rowsElement) && rowsElement.GetArrayLength() > 0)
            {
                var firstRow = rowsElement[0];
                if (firstRow.TryGetProperty("elements", out JsonElement elementsElement) && elementsElement.GetArrayLength() > 0)
                {
                    var firstElement = elementsElement[0];
                    if (firstElement.TryGetProperty("distance", out JsonElement distanceElement))
                    {
                        var distanceInMeters = distanceElement.GetProperty("value").GetDouble();
                        return distanceInMeters / 1000.0; // distance in kilometers
                    }
                }
            }
            throw new Exception("Failed to calculate walking distance.");
        }
        else
        {
            throw new Exception($"Failed to call API: {response.ReasonPhrase}");
        }
    }

    public static (double Latitude, double Longitude) GetCoordinates(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be null or empty.");
        }

        var apiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4"; 
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


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
    private const string ApiKey = "675ef7e408d33282453687qrh963303";
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
    public static double CalculateDrivingDistance(double lat1, double lon1, double lat2, double lon2)
    {
        using HttpClient client = new HttpClient();
        string requestUrl = $"http://router.project-osrm.org/route/v1/driving/{lon1},{lat1};{lon2},{lat2}?overview=false";

        try
        {
            var response = client.GetAsync(requestUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                using var jsonDocument = JsonDocument.Parse(content);

                var root = jsonDocument.RootElement;
                if (root.TryGetProperty("routes", out JsonElement routesElement) && routesElement.GetArrayLength() > 0)
                {
                    var firstRoute = routesElement[0];
                    if (firstRoute.TryGetProperty("distance", out JsonElement distanceElement))
                    {
                        var distanceInMeters = distanceElement.GetDouble();
                        return distanceInMeters / 1000.0; // המרחק בקילומטרים
                    }
                }
            }
            throw new Exception("Failed to calculate driving distance.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }

    // חישוב מרחק בהליכה בין שתי נקודות קואורדינטות
    public static double CalculateWalkingDistance(double lat1, double lon1, double lat2, double lon2)
    {
        using HttpClient client = new HttpClient();
        string requestUrl = $"http://router.project-osrm.org/route/v1/foot/{lon1},{lat1};{lon2},{lat2}?overview=false";

        try
        {
            var response = client.GetAsync(requestUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                using var jsonDocument = JsonDocument.Parse(content);

                var root = jsonDocument.RootElement;
                if (root.TryGetProperty("routes", out JsonElement routesElement) && routesElement.GetArrayLength() > 0)
                {
                    var firstRoute = routesElement[0];
                    if (firstRoute.TryGetProperty("distance", out JsonElement distanceElement))
                    {
                        var distanceInMeters = distanceElement.GetDouble();
                        return distanceInMeters / 1000.0; // המרחק בקילומטרים
                    }
                }
            }
            throw new Exception("Failed to calculate walking distance.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }
    public static (double Latitude, double Longitude) GetCoordinates(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be null or empty.");
        }

        // URL מותאם עבור ה-API של Maps.co (הוספתי את המפתח בהתאם)
        var url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={ApiKey}";

        using (var client = new HttpClient())
        {
            var response = client.GetStringAsync(url).Result;

            // ניתוח התשובה ב-JSON
            var jsonResponse = JsonDocument.Parse(response);

            // אם יש תוצאות בתשובה
            if (jsonResponse.RootElement.GetArrayLength() > 0)
            {
                // הפנייה לתוצאה הראשונה
                var firstResult = jsonResponse.RootElement[0];

                // חילוץ הקואורדינטות
                var latitude = firstResult.GetProperty("lat").GetString();
                var longitude = firstResult.GetProperty("lon").GetString();

                // המרה ל-double
                if (double.TryParse(latitude, out double lat) && double.TryParse(longitude, out double lon))
                {
                    return (lat, lon);
                }
                else
                {
                    throw new Exception("Failed to parse latitude or longitude.");
                }
            }
            else
            {
                throw new Exception("No results found for the given address.");
            }
        }
    }



}

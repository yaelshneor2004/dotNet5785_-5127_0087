
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
    public static double GlobalDistance(string volunterrAddress, string callAddress, DO.MyTypeDistance myTypeDistance)
    {
        return myTypeDistance switch
        {
            DO.MyTypeDistance.Aerial => CalculateAerialDistance(callAddress,volunterrAddress),
            DO.MyTypeDistance.Walking => CalcWalkingDistance(callAddress,volunterrAddress),
            DO.MyTypeDistance.Traveling => CalcDrivingDistance(callAddress,volunterrAddress),
            _ => throw new BlInvalidOperationException("Invalid distance type")
        };
    }
    private static double CalculateAerialDistance(string volunteerAddress, string callAddress)
    {
        // Get coordinates of the volunteer's address
        var (volunteerLat, volunteerLon) = GetCoordinates(volunteerAddress);

        // Get coordinates of the call's address
        var (callLat, callLon) = GetCoordinates(callAddress);

        // Calculate the aerial distance between two coordinates using the Haversine formula
        const double R = 6371; // Radius of the Earth in kilometers
        var lat = ToRadians(callLat - volunteerLat);
        var lon = ToRadians(callLon - volunteerLon);
        var a = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                Math.Cos(ToRadians(volunteerLat)) * Math.Cos(ToRadians(callLat)) *
                Math.Sin(lon / 2) * Math.Sin(lon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double angle)
    {
        return angle * (Math.PI / 180);
    }


    private static double CalcWalkingDistance(string callAddress, string volunteerAddress)
    {
        using (var client = new HttpClient())
        {
            string apiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4";
            string requestUri = $"https://maps.googleapis.com/maps/api/distancematrix/xml?origins={Uri.EscapeDataString(volunteerAddress)}&destinations={Uri.EscapeDataString(callAddress)}&mode=walking&units=metric&key={apiKey}";

            var response = client.GetAsync(requestUri).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(content);
                var distanceNode = xmlDoc.SelectSingleNode("//DistanceMatrixResponse/row/element/distance/value");
                if (distanceNode != null)
                {
                    double distance = double.Parse(distanceNode.InnerText);
                    return distance / 1000; // Convert meters to kilometers
                }
                else
                {
                    throw new BlInvalidOperationException("Unable to calculate walking distance using Google Distance Matrix API.");
                }
            }
            else
            {
                throw new BlInvalidOperationException("Unable to calculate walking distance using Google Distance Matrix API.");
            }
        }
    }



    internal static double CalcDrivingDistance(string callAddress, string volunteerAddress)
    {
        using (var client = new HttpClient())
        {
            string apiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4";
            string requestUri = $"https://maps.googleapis.com/maps/api/distancematrix/xml?origins={Uri.EscapeDataString(volunteerAddress)}&destinations={Uri.EscapeDataString(callAddress)}&units=metric&key={apiKey}";

            var response = client.GetAsync(requestUri).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(content);
                var distanceNode = xmlDoc.SelectSingleNode("//DistanceMatrixResponse/row/element/distance/value");
                if (distanceNode != null)
                {
                    double distance = double.Parse(distanceNode.InnerText);
                    return distance / 1000; // Convert meters to kilometers
                }
                else
                {
                    throw new BlInvalidOperationException("Unable to calculate driving distance using Google Distance Matrix API.");
                }
            }
            else
            {
                throw new BlInvalidOperationException("Unable to calculate driving distance using Google Distance Matrix API.");
            }
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
    public static IEnumerable<BO.Volunteer> GetVolunteersWithinDistance(string callAddress)
    {
        var volunteers = s_dal.Volunteer.ReadAll();

        var newVolunteers = volunteers.Where(volunteer =>
          {
              if (volunteer.Latitude == null || volunteer.Longitude == null)
              {
                  return false; 
              }
              var distance = CalculateAerialDistance(volunteer.Address,callAddress);
              return distance <= volunteer.MaxDistance;
          });
        return newVolunteers.Select(v => VolunteerManager.ConvertFromDoToBo(v)).ToList();
    }
}


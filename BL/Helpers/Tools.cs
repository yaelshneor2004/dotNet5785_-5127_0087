
using BO;
using DalApi;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

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
    public static double GlobalDistance(double? lat1, double? lon1, double lat2, double lon2, DO.MyTypeDistance myTypeDistance)
    {
        if (lat1 == null || lon1 == null)
            throw new BlInvalidOperationException("Latitude and Longitude cannot be null.");
        return myTypeDistance switch
        {
            DO.MyTypeDistance.Aerial => CalculateAerialDistance(lat1.Value, lon1.Value, lat2, lon2),
            DO. MyTypeDistance.Walking => CalculateWalkingDistance(lat1.Value, lon1.Value, lat2, lon2),
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
    private static double CalculateDrivingDistance(double? lat1, double? lon1, double lat2, double lon2)
    {
        var client = new HttpClient();
        var response = client.GetAsync($"https://routing.openstreetmap.de/routed-car/route/v1/driving/{lon1},{lat1};{lon2},{lat2}?overview=false").Result;

        if (response.IsSuccessStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            dynamic result = JsonConvert.DeserializeObject(content);

            if (result != null && result.routes != null && result.routes.Count > 0)
            {
                var distanceInMeters = result.routes[0].distance;
                return distanceInMeters / 1000.0;
            }
        }

        throw new BlInvalidOperationException("Failed to calculate driving distance.");
    }
    private static double CalculateWalkingDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var client = new HttpClient();
        var response = client.GetAsync($"https://routing.openstreetmap.de/routed-foot/route/v1/driving/{lon1},{lat1};{lon2},{lat2}?overview=false").Result;

        if (response.IsSuccessStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            dynamic result = JsonConvert.DeserializeObject(content);

            if (result != null && result.routes != null && result.routes.Count > 0)
            {
                var distanceInMeters = result.routes[0].distance;
                return distanceInMeters / 1000.0;
            }
        }
        throw new BlInvalidOperationException("Failed to calculate walking distance.");
    }
}

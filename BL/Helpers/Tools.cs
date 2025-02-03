using BO;
using DalApi;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
namespace Helpers;
internal static class Tools
{
    private const string apiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4";

    private static IDal s_dal = Factory.Get;
    /// <summary>
    /// Calculates the global distance between two addresses based on the specified distance type.
    /// </summary>
    /// <param name="volunterrAddress">The address of the volunteer.</param>
    /// <param name="callAddress">The address of the call.</param>
    /// <param name="myTypeDistance">The type of distance calculation (Aerial, Walking, Traveling).</param>
    /// <returns>The calculated distance.</returns>
    public static double GlobalDistance(string volunterrAddress, string callAddress, DO.MyTypeDistance myTypeDistance)
    {
        return myTypeDistance switch
        {
            DO.MyTypeDistance.Aerial => CalculateAerialDistance(callAddress, volunterrAddress),
            DO.MyTypeDistance.Walking => CalcWalkingDistance(callAddress, volunterrAddress),
            DO.MyTypeDistance.Traveling => CalcDrivingDistance(callAddress, volunterrAddress),
            _ => throw new BO.BlInvalidOperationException("Invalid distance type")
        };
    }

    /// <summary>
    /// Calculates the aerial distance between two addresses using the Haversine formula.
    /// </summary>
    /// <param name="volunteerAddress">The address of the volunteer.</param>
    /// <param name="callAddress">The address of the call.</param>
    /// <returns>The aerial distance in kilometers.</returns>
    private static double CalculateAerialDistance(string volunteerAddress, string callAddress)
    {
        // Get coordinates of the volunteer's address
        var (volunteerLat, volunteerLon) = GetCoordinates(volunteerAddress).Result;

        // Get coordinates of the call's address
        var (callLat, callLon) = GetCoordinates(callAddress).Result;

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
    ///// <summary>
    ///// Converts an angle from degrees to radians.
    ///// </summary>
    ///// <param name="angle">The angle in degrees.</param>
    ///// <returns>The angle in radians.</returns>
    private static double ToRadians(double angle)
    {
        return angle * (Math.PI / 180);
    }

    ///// <summary>
    ///// Calculates the walking distance between two addresses using Google Distance Matrix API.
    ///// </summary>
    ///// <param name="callAddress">The address of the call.</param>
    ///// <param name="volunteerAddress">The address of the volunteer.</param>
    ///// <returns>The walking distance in kilometers.</returns>
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
                    throw new BO.BlInvalidOperationException("Unable to calculate walking distance using Google Distance Matrix API.");
                }
            }
            else
            {
                throw new BO.BlInvalidOperationException("Unable to calculate walking distance using Google Distance Matrix API.");
            }
        }
    }

    ///// <summary>
    ///// Calculates the driving distance between two addresses using Google Distance Matrix API.
    ///// </summary>
    ///// <param name="callAddress">The address of the call.</param>
    ///// <param name="volunteerAddress">The address of the volunteer.</param>
    ///// <returns>The driving distance in kilometers.</returns>
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
                    throw new BO.BlInvalidOperationException("Unable to calculate driving distance using Google Distance Matrix API.");
                }
            }
            else
            {
                throw new BO.BlInvalidOperationException("Unable to calculate driving distance using Google Distance Matrix API.");
            }
        }
    }
    /// <summary>
    /// Retrieves the coordinates (latitude and longitude) of a given address using Google Maps Geocoding API.
    /// </summary>
    /// <param name="address">The address to get coordinates for.</param>
    /// <returns>A tuple containing the latitude and longitude of the address.</returns>
    /// <exception cref="BlInvalidOperationException">Thrown when the coordinates cannot be retrieved or an error occurs.</exception>
    internal static async Task<(double Latitude, double Longitude)> GetCoordinates(string address)
    {
        try
        {
            using (var client = new HttpClient())
            {
                string apiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4";
                string requestUri = $"https://maps.googleapis.com/maps/api/geocode/xml?address={Uri.EscapeDataString(address)}&key={apiKey}";
                var response = await client.GetAsync(requestUri).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var xmlDoc = new System.Xml.XmlDocument();
                    xmlDoc.LoadXml(content);
                    var locationNode = xmlDoc.SelectSingleNode("//GeocodeResponse/result/geometry/location");
                    if (locationNode != null)
                    {
                        var latNode = locationNode.SelectSingleNode("lat");
                        var lngNode = locationNode.SelectSingleNode("lng");
                        if (latNode != null && lngNode != null)
                        {
                            double latitude = double.Parse(latNode.InnerText);
                            double longitude = double.Parse(lngNode.InnerText);
                            return (latitude, longitude);
                        }
                        else
                        {
                            throw new BlInvalidOperationException("Unable to get coordinates from Google Maps API.");
                        }
                    }
                    else
                    {
                        throw new BlInvalidOperationException("Unable to get coordinates from Google Maps API.");
                    }
                }
                else
                {
                    throw new BlInvalidOperationException("Unable to get coordinates from Google Maps API.");
                }
            }
        }
        catch (HttpRequestException)
        {
            throw new BlInvalidOperationException("HTTP request failed.");
        }
        catch (System.Xml.XmlException)
        {
            throw new BlInvalidOperationException("XML parsing failed.");
        }
        catch (Exception ex)
        {
            throw new BlInvalidOperationException("An unexpected error occurred: " + ex.Message);
        }
    }
    /// <summary>
    /// Validates if the provided address is valid using Google Maps API.
    /// </summary>
    internal static bool IsValidAddress(string? address)
    {
        if (string.IsNullOrWhiteSpace(address)) return true;
        using (var client = new HttpClient())
        {
            string apiKey = "AIzaSyDp5JA_AxKyCcz9QK9q1btolMB6Y8jusc4";
            string requestUri = $"https://maps.googleapis.com/maps/api/geocode/xml?address={Uri.EscapeDataString(address)}&key={apiKey}";

            var response = client.GetAsync(requestUri).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(responseContent);
                var statusNode = xmlDoc.SelectSingleNode("//GeocodeResponse/status");
                if (statusNode != null && statusNode.InnerText == "OK")
                {
                    return true;
                }
                throw new BlInvalidOperationException("Invalid address.");
            }
            else
            {
                throw new BlInvalidOperationException("Unable to validate address using Google Maps API.");
            }
        }
    }
}
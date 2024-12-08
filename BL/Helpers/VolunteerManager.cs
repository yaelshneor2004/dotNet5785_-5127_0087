using System;
using System.Net.Http;
using Newtonsoft.Json;


using DalApi;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get;
    public static double CalculateDistance(double? lat1, double? lon1, double lat2, double lon2)
    {
        if (lat1 == null || lon1 == null)
        {
            throw new BO.BlNullPropertyException("Latitude and Longitude cannot be null.");
        }

        const double R = 6371e3; // Radius of the Earth in meters
        var phi1 = lat1.Value * Math.PI / 180; // φ, λ in radians
        var phi2 = lat2 * Math.PI / 180;
        var deltaPhi = (lat2 - lat1.Value) * Math.PI / 180;
        var deltaLambda = (lon2 - lon1.Value) * Math.PI / 180;

        var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                Math.Cos(phi1) * Math.Cos(phi2) *
                Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var distance = R * c; // in meters

        return distance;
    }
    public static BO.MyCallStatusByVolunteer DetermineCallStatus(DateTime? maxFinishTime)
    {
        // Use the updated system clock from ClockManager
        var remainingTime = maxFinishTime - ClockManager.Now;

        if (remainingTime <= s_dal.Config.RiskRange)
        {
            return BO.MyCallStatusByVolunteer.AtRisk;
        }

        return BO.MyCallStatusByVolunteer.InProgress; 
    }



        public static void ValidateVolunteerDetails(BO.Volunteer volunteer)
        {
        // Email format check
        if (!IsValidEmail(volunteer.Email))
            {
                throw new BO.BlException("Invalid email format.");
            }

        // Check if the numeric fields are indeed numeric
        if (!IsNumeric(volunteer.Id.ToString()))
        {
            throw new BO.BlException("Invalid ID format.");
            }

            // בדיקה אם כתובת תקינה (אפשר להשתמש בשירותי API למשל)
            if (!IsValidAddress(volunteer.Address, out double latitude, out double longitude))
            {
                throw new BO.BlException("Invalid address.");
            }

        // Update the longitude and latitude by address
        volunteer.Latitude = latitude;
        volunteer.Longitude = longitude;

            // בדיקות נוספות לפי הצורך...
        }

        private static bool IsValidEmail(string email)
        {
        // Email format check
        return email.Contains("@") && email.Contains(".");
    }

    private static bool IsNumeric(string value)
        {
            return int.TryParse(value, out _);
        }

   

    private static bool IsValidAddress(string address, out double latitude, out double longitude)
        {
            latitude = 0;
            longitude = 0;

            var client = new HttpClient();
            var response = client.GetAsync($"https://nominatim.openstreetmap.org/search?q={address}&format=json&addressdetails=1").Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                dynamic result = JsonConvert.DeserializeObject(content);

                if (result != null && result.Count > 0)
                {
                    latitude = result[0].lat;
                    longitude = result[0].lon;
                    return true;
                }
            }

            return false;
        
        }


}



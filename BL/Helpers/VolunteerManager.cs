using System;
using System.Net.Http;
using Newtonsoft.Json;
using DalApi;
using System.Text.RegularExpressions;

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
        // Check if the email format is valid
        if (!IsValidEmail(volunteer.Email))
        {
            throw new BO.BlException("Invalid email format.");
        }

        // Check if the ID is numeric and valid
        if (!IsNumeric(volunteer.Id.ToString()) || !ValidateIdNumber(volunteer.Id.ToString()))
        {
            throw new BO.BlException("Invalid ID format.");
        }

        // Check if the address is valid (can use API services)
        if (!IsValidAddress(volunteer.Address, out double latitude, out double longitude))
        {
            throw new BO.BlException("Invalid address.");
        }

        // Update the longitude and latitude based on the address
        volunteer.Latitude = latitude;
        volunteer.Longitude = longitude;
    }

    // Method to validate the logical correctness of the email
    private static bool IsValidEmail(string email)
    {
        // Regular expression pattern for validating email
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // Use Regex to check if the email matches the pattern
        return Regex.IsMatch(email, emailPattern);
    }

    // Check if the value is numeric
    private static bool IsNumeric(string value)
    {
        return int.TryParse(value, out _);
    }

    // Validate the logical correctness of the ID number
    public static bool ValidateIdNumber(string idNumber)
    {
        // Check if the ID number contains exactly 9 digits
        if (idNumber.Length != 9 || !idNumber.All(char.IsDigit))
        {
            return false;
        }

        // Calculate the checksum digit
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = idNumber[i] - '0'; // Convert character to number
            int weight = i % 2 == 0 ? 1 : 2; // Weight: 1 for even positions, 2 for odd positions
            int product = digit * weight;

            // If the product is greater than 9, sum the digits of the product
            sum += product > 9 ? product - 9 : product;
        }

        // Check if the checksum is valid
        return sum % 10 == 0;
    }

    // Validate the address and get latitude and longitude
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



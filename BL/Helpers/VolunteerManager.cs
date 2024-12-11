using System;
using System.Net.Http;
using Newtonsoft.Json;
using DalApi;
using System.Text.RegularExpressions;
using BO;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get;
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
        // Check if the email format and logical are valid
        if (!IsValidEmail(volunteer.Email))
            throw new BO.BlInvalidOperationException("Invalid email format.");
        // Check if the ID is numeric and valid
        if (!IsNumeric(volunteer.Id.ToString()) || !ValidateIdNumber(volunteer.Id.ToString()))
            throw new BO.BlInvalidOperationException("Invalid ID format.");
        // Check if the address is valid (can use API services)
        if (!IsValidAddress(volunteer.Address, out double latitude, out double longitude)) 
            throw new BO.BlInvalidOperationException("Invalid address.");
        volunteer.Latitude = latitude;
        volunteer.Longitude = longitude;  
            if (!IsValidPhoneNumber(volunteer.Phone))
            throw new BO.BlInvalidOperationException($"Invalid phone {volunteer.Phone}.");
        if(!IsValidFirstName(volunteer.FullName))
            throw new BO.BlInvalidOperationException($"Invalid name {volunteer.FullName}.");
        if (!IsStrongPassword(volunteer.Password))
            throw new BO.BlInvalidOperationException($"this Password is not strong enough.");
        // Update the longitude and latitude based on the address

    }
    private static bool IsStrongPassword(string password)
    {
        if (password.Length < 8) // Check length
            return false;

        if (!Regex.IsMatch(password, "[A-Z]")) // Check for uppercase letter
            return false;

        if (!Regex.IsMatch(password, "[a-z]")) // Check for lowercase letter
            return false;

        if (!Regex.IsMatch(password, "[0-9]")) // Check for digit
            return false;

        if (!Regex.IsMatch(password, "[!@#$%^&*(),.?\":{}|<>]")) // Check for special character
            return false;

        return true;
    }
// Method to validate the logical correctness of the email
private static bool IsValidEmail(string email)
    {
        // Regular expression pattern for validating email
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // Use Regex to check if the email matches the pattern
        return Regex.IsMatch(email, emailPattern);
    }
   private static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Regular expression pattern for validating phone number
        string phonePattern = @"^\+?[0-9]{1,4}?[-.\s]?(\(?\d{1,3}?\))?[-.\s]?\d{1,4}[-.\s]?\d{1,4}[-.\s]?\d{1,9}$";

        // Use Regex to check if the phone number matches the pattern
        if (!Regex.IsMatch(phoneNumber, phonePattern))
        {
            return false;
        }

        // Additional logical checks
        // Check if phone number has valid length (typically between 10 to 15 digits)
        var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
        if (digitsOnly.Length < 10 || digitsOnly.Length > 15)
        {
            return false;
        }
        return true;
    }
    // Check if the value is numeric
    private static bool IsNumeric(string value)
    {
        return int.TryParse(value, out _);
    }
    // Validate the logical correctness of the ID number
    private static bool ValidateIdNumber(string idNumber)
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
    private static bool IsValidFirstName(string firstName)
    {
        // Regular expression pattern for validating a first name
        string firstNamePattern = @"^[A-Za-zא-ת]{2,50}$";

        // Use Regex to check if the first name matches the pattern
        if (!Regex.IsMatch(firstName, firstNamePattern))
        {
            return false;
        }

        // Additional logical checks (if needed)
        // Check if the first name is not empty and has a valid length (typically between 2 to 50 characters)
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 50)
        {
            return false;
        }

        return true;
    }
  
    // Helper method to sort the volunteer list
    public static IEnumerable<BO.VolunteerInList> SortVolunteers(IEnumerable<BO.VolunteerInList> volunteers, BO.MySortInVolunteerInList sortBy)
    {
        return sortBy switch
        {
            BO.MySortInVolunteerInList.FullName => volunteers.OrderBy(v => v.FullName),
            BO.MySortInVolunteerInList.TotalCallsHandled => volunteers.OrderBy(v => v.TotalCallsHandled),
            BO.MySortInVolunteerInList.TotalCallsCancelled => volunteers.OrderBy(v => v.TotalCallsCancelled),
            BO.MySortInVolunteerInList.CurrentCallType => volunteers.OrderBy(v => v.CurrentCallType),
            _ => volunteers.OrderBy(v => v.Id)
        };
    }

}



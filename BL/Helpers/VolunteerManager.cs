using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using BlApi;
using DalApi;
using Newtonsoft.Json;
using static Helpers.VolunteerManager;

namespace Helpers;
internal static class VolunteerManager
{
    private static IDal s_dal = DalApi. Factory.Get;
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789ABCDEF"); // מפתח של 16 בתים
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("ABCDEF9876543210");  // וקטור אתחול של 16 בתים

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
        // Check if the ID is numeric and valid
        if (!IsNumeric(volunteer.Id.ToString()) /*||*/ /*!ValidateIdNumber(volunteer.Id.ToString())*/)
            throw new BO.BlInvalidOperationException("Invalid ID format.");
        if (!IsValidFirstName(volunteer.FullName))
            throw new BO.BlInvalidOperationException($"Invalid name {volunteer.FullName}.");
        if (!IsValidPhoneNumber(volunteer.Phone))
            throw new BO.BlInvalidOperationException($"Invalid phone {volunteer.Phone}.");
        // Check if the email format and logical are valid
        if (!IsValidEmail(volunteer.Email))
            throw new BO.BlInvalidOperationException("Invalid email format.");
        if (!IsStrongPassword(volunteer.Password))
            throw new BO.BlInvalidOperationException($"this Password is not strong enough.");
         var coordinates = Tools.GetCoordinates(volunteer.Address);
        volunteer.Latitude = coordinates.Latitude;
    volunteer.Longitude = coordinates.Longitude;
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
        if (idNumber.Length != 8 || !idNumber.All(char.IsDigit))
        {
            return false;
        }
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            int digit = idNumber[i] - '0'; 
            int weight = (i % 2) + 1; 
            int product = digit * weight;
            sum += product > 9 ? product - 9 : product;
        }

        int checkDigit = sum % 10 == 0 ? 0 : 10 - (sum % 10);
        return checkDigit == (idNumber[8] - '0');
    }

public static bool IsValidFirstName(string name)
    {
        foreach (char c in name)
        {
            if (!char.IsLetter(c) && c != ' ')
            {
                return false;
            }
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
    public static DO.Volunteer ConvertFromBoToDo(BO.Volunteer myVolunteer)
    {
         return new DO.Volunteer
        {
            Id = myVolunteer.Id,
            FullName = myVolunteer.FullName,
            Phone = myVolunteer.Phone,
            Email = myVolunteer.Email,
            Password = myVolunteer.Password,
            Address = myVolunteer.Address,
            Latitude = myVolunteer.Latitude,
            Longitude = myVolunteer.Longitude,
            IsActive = myVolunteer.IsActive,
            MaxDistance = myVolunteer.MaxDistance,
            TypeDistance = (DO.MyTypeDistance)myVolunteer.TypeDistance,
            Role = (DO.MyRole)myVolunteer.Role
        };
    }
    public static BO.Volunteer ConvertFromDoToBo(DO.Volunteer myVolunteer)
    {
        var assignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == myVolunteer.Id).ToList();



        return new BO.Volunteer
        (
            id: myVolunteer.Id,
            fullName: myVolunteer.FullName,
            phone: myVolunteer.Phone,
            email: myVolunteer.Email,
            password: VolunteerManager.Decrypt( myVolunteer.Password) ?? null,
            address: myVolunteer.Address ?? null,
            latitude: myVolunteer.Latitude ?? null,
            longitude: myVolunteer.Longitude ?? null,
            role: (BO.MyRole)myVolunteer.Role,
            isActive: myVolunteer.IsActive,
            maxDistance: myVolunteer.MaxDistance ?? null,
            typeDistance: (BO.MyTypeDistance)myVolunteer.TypeDistance,
            totalCallsHandled: s_dal.Assignment.ReadAll(a => a.VolunteerId == myVolunteer.Id && a.FinishType == DO.MyFinishType.Treated).Count(),
            totalCallsCancelled: s_dal.Assignment.ReadAll(a => a.VolunteerId == myVolunteer.Id && (a.FinishType == DO.MyFinishType.SelfCancel || a.FinishType == DO.MyFinishType.ManagerCancel)).Count(),
            totalCallsExpired: s_dal.Assignment.ReadAll(a => a.VolunteerId == myVolunteer.Id && a.FinishType == DO.MyFinishType.ExpiredCancel).Count(),
            currentCall: (from a in assignments
                          let callData = s_dal.Call.Read(a.CallId)
                          select new BO.CallInProgress
                          {
                              Id = a.Id,
                              CallId = a.CallId,
                              CallType = (BO.MyCallType)callData.CallType,
                              Description = callData.Description,
                              Address = callData.Address,
                              StartTime = callData.OpenTime,
                              MaxEndTime = callData.MaxFinishCall,
                              StartTreatmentTime = a.StartCall,
                              DistanceFromVolunteer = Tools.GlobalDistance(myVolunteer.Address, callData.Address,  myVolunteer.TypeDistance),
                              Status = VolunteerManager.DetermineCallStatus(callData.MaxFinishCall)
                          }).FirstOrDefault() // Assuming CurrentCall should be the first open call or null
        );
    }

    public static BO.VolunteerInList ConvertToVolunteerInList(DO.Volunteer VolunteerData)
    {
        return new BO.VolunteerInList
        {
            Id = VolunteerData.Id,
            FullName = VolunteerData.FullName,
            IsActive = VolunteerData.IsActive,
            TotalCallsHandled = s_dal.Assignment.ReadAll(a => a.VolunteerId == VolunteerData.Id && a.FinishType == DO.MyFinishType.Treated).Count(),
            TotalCallsCancelled = s_dal.Assignment.ReadAll(a => a.VolunteerId == VolunteerData.Id && (a.FinishType == DO.MyFinishType.SelfCancel || a.FinishType == DO.MyFinishType.ManagerCancel)).Count(),
            TotalCallsExpired = s_dal.Assignment.ReadAll(a => a.VolunteerId == VolunteerData.Id && a.FinishType == DO.MyFinishType.ExpiredCancel).Count(),
            CurrentCallId = s_dal.Assignment.ReadAll(a => a.VolunteerId == VolunteerData.Id && a.FinishCall == null).Select(a => (int?)a.CallId).FirstOrDefault(),
            CurrentCallType = s_dal.Assignment.ReadAll(a => a.VolunteerId == VolunteerData.Id && a.FinishCall == null)
           .Select(a => s_dal.Call.ReadAll(c => c.Id == a.CallId).Select(c => (BO.MyCurrentCallType?)c.CallType).FirstOrDefault()).FirstOrDefault() ?? BO.MyCurrentCallType.None
        };
    }

  public static void PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock)
        {
            var volunteers = s_dal.Volunteer.ReadAll().ToList();
            var assignments = s_dal.Assignment.ReadAll().ToList();

            var volunteerUpdates = volunteers.Select(volunteer =>
            {
                var volunteerAssignments = assignments.Where(a => a.VolunteerId == volunteer.Id).ToList();

                // If the volunteer has not handled calls for 2 years, they are marked as inactive
                if (!volunteerAssignments.Any() || (newClock - volunteerAssignments.Max(a => a.FinishCall ?? DateTime.MinValue)).TotalDays > 2 * 365)
                {
                    volunteer = volunteer with { IsActive = false };
                }

                // Upgrade the volunteer's role if he has handled more than 100 calls and is not a manager
                if (volunteerAssignments.Count(a => a.FinishType == DO.MyFinishType.Treated) >= 100 && volunteer.Role != DO.MyRole.Manager)
                {
                    volunteer = volunteer with { Role = DO.MyRole.Manager };
                }

                return volunteer;
            }).ToList();

        // Update volunteers in the database
        volunteerUpdates.ForEach(volunteer => s_dal.Volunteer.Update(volunteer));
    }
    public static string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }
    }
    public static string Decrypt(string encryptedText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }

}




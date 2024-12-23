using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using DalApi;
namespace Helpers;
internal static class VolunteerManager
{
    internal static ObserverManager Observers = new(); 

    private static IDal s_dal = DalApi.Factory.Get;
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789ABCDEF"); // 16-byte key
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("ABCDEF9876543210");  // 16-byte initialization vector

    /// <summary>
    /// Determines the call status based on the remaining time until max finish time.
    /// </summary>
    /// <param name="maxFinishTime">The maximum finish time for the call.</param>
    /// <returns>Returns the call status by volunteer (InProgress or AtRisk).</returns>
    public static BO.MyCallStatusByVolunteer DetermineCallStatus(DateTime? maxFinishTime)
    {
        // Use the updated system clock from ClockManager
        var remainingTime = maxFinishTime - AdminManager.Now;

        if (remainingTime <= s_dal.Config.RiskRange)
        {
            return BO.MyCallStatusByVolunteer.AtRisk;
        }

        return BO.MyCallStatusByVolunteer.InProgress;
    }

    /// <summary>
    /// Validates the details of a volunteer.
    /// </summary>
    /// <param name="volunteer">The volunteer to validate.</param>
    public static void ValidateVolunteerDetails(BO.Volunteer volunteer)
    {
        // Check if the ID is numeric and valid
        if (!IsNumeric(volunteer.Id.ToString()) || !ValidateIdNumber(volunteer.Id.ToString()))
            throw new BO.BlInvalidOperationException("Invalid ID format.");
        if (!IsValidFirstName(volunteer.FullName))
            throw new BO.BlInvalidOperationException($"Invalid name {volunteer.FullName}.");
        if (!IsValidPhoneNumber(volunteer.Phone))
            throw new BO.BlInvalidOperationException($"Invalid phone {volunteer.Phone}.");
        // Check if the email format and logical are valid
        if (!IsValidEmail(volunteer.Email))
            throw new BO.BlInvalidOperationException("Invalid email format.");
        if (!IsStrongPassword(volunteer.Password ?? string.Empty))
            throw new BO.BlInvalidOperationException("This password is not strong enough.");
        var coordinates = Tools.GetCoordinates(volunteer.Address ?? string.Empty);
        volunteer.Latitude = coordinates.Latitude;
        volunteer.Longitude = coordinates.Longitude;
    }

    /// <summary>
    /// Checks if the given password is strong.
    /// </summary>
    /// <param name="password">The password to check.</param>
    /// <returns>Returns true if the password is strong, otherwise false.</returns>
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

    /// <summary>
    /// Method to validate the logical correctness of the email.
    /// </summary>
    /// <param name="email">The email to validate.</param>
    /// <returns>Returns true if the email is valid, otherwise false.</returns>
    private static bool IsValidEmail(string email)
    {
        // Regular expression pattern for validating email
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // Use Regex to check if the email matches the pattern
        return Regex.IsMatch(email, emailPattern);
    }

    /// <summary>
    /// Validates the phone number format and logical correctness.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate.</param>
    /// <returns>Returns true if the phone number is valid, otherwise false.</returns>
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

    /// <summary>
    /// Check if the value is numeric.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>Returns true if the value is numeric, otherwise false.</returns>
    private static bool IsNumeric(string value)
    {
        return int.TryParse(value, out _);
    }

    /// <summary>
    /// Validates the logical correctness of the ID number.
    /// </summary>
    /// <param name="idNumber">The ID number to validate.</param>
    /// <returns>Returns true if the ID number is valid, otherwise false.</returns>
    private static bool ValidateIdNumber(string idNumber)
    {
        // Check that the string length is exactly 9 characters and contains only digits
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

    /// <summary>
    /// Checks if the name is valid.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>Returns true if the name is valid, otherwise false.</returns>
    public static bool IsValidFirstName(string name)
    {
        // Iterate through each character in the name
        foreach (char c in name)
        {
            // Check if the character is not a letter and not a space
            if (!char.IsLetter(c) && c != ' ')
            {
                return false;
            }
        }
        return true; // All characters are valid
    }

    /// <summary>
    /// Helper method to sort the volunteer list.
    /// </summary>
    /// <param name="volunteers">The list of volunteers.</param>
    /// <param name="sortBy">The sorting criteria.</param>
    /// <returns>Returns the sorted list of volunteers.</returns>
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

    /// <summary>
    /// Converts a BO.Volunteer to a DO.Volunteer.
    /// </summary>
    /// <param name="myVolunteer">The BO.Volunteer object to convert.</param>
    /// <returns>Returns the converted DO.Volunteer object.</returns>
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
    /// <summary>
    /// Converts a DO.Volunteer to a BO.Volunteer.
    /// </summary>
    /// <param name="myVolunteer">The DO.Volunteer object to convert.</param>
    /// <returns>Returns the converted BO.Volunteer object.</returns>
    public static BO.Volunteer ConvertFromDoToBo(DO.Volunteer myVolunteer)
    {
        var assignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == myVolunteer.Id).ToList();

        return new BO.Volunteer
        (
            id: myVolunteer.Id,
            fullName: myVolunteer.FullName,
            phone: myVolunteer.Phone,
            email: myVolunteer.Email,
            password: VolunteerManager.Decrypt(myVolunteer.Password ?? string.Empty) ?? null,
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
                              DistanceFromVolunteer = Tools.GlobalDistance(myVolunteer.Address ?? string.Empty, callData.Address, myVolunteer.TypeDistance),
                              Status = VolunteerManager.DetermineCallStatus(callData.MaxFinishCall)
                          }).FirstOrDefault() // Assuming CurrentCall should be the first open call or null
        );
    }

    /// <summary>
    /// Converts a DO.Volunteer to a BO.VolunteerInList.
    /// </summary>
    /// <param name="VolunteerData">The DO.Volunteer object to convert.</param>
    /// <returns>Returns the converted BO.VolunteerInList object.</returns>
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

    /// <summary>
    /// Periodically updates volunteers' status and roles based on their activities.
    /// </summary>
    /// <param name="oldClock">The previous clock time.</param>
    /// <param name="newClock">The new clock time.</param>
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
                Observers.NotifyItemUpdated(volunteer.Id); // Add call to NotifyItemUpdated
            }

            // Upgrade the volunteer's role if he has handled more than 100 calls and is not a manager
            if (volunteerAssignments.Count(a => a.FinishType == DO.MyFinishType.Treated) >= 100 && volunteer.Role != DO.MyRole.Manager)
            {
                volunteer = volunteer with { Role = DO.MyRole.Manager };
                Observers.NotifyItemUpdated(volunteer.Id); // Add call to NotifyItemUpdated
            }

            return volunteer;
        }).ToList();

        // Update volunteers in the database
        volunteerUpdates.ForEach(volunteer => s_dal.Volunteer.Update(volunteer));

        // Add call to NotifyListUpdated
        Observers.NotifyListUpdated();
    }



    /// <summary>
    /// Encrypts a plain text string using AES encryption.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <returns>Returns the encrypted text as a base64 encoded string.</returns>
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

    /// <summary>
    /// Decrypts an encrypted text string using AES decryption.
    /// </summary>
    /// <param name="encryptedText">The encrypted text to decrypt.</param>
    /// <returns>Returns the decrypted plain text string.</returns>
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

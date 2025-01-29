using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using BO;
using DalApi;
using DO;
namespace Helpers;
internal static class VolunteerManager
{
    internal static ObserverManager Observers = new();

    private static readonly Random s_rand = new(); // Random number generator
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
        if (!IsNumeric(volunteer.Id.ToString()) || !IsValidID(volunteer.Id))
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
        if (!Tools.IsValidAddress(volunteer.Address))
            throw new BO.BlInvalidOperationException("invalid Address");

    }
    public static async Task AddVolunteerCoordinatesAsync(BO.Volunteer volunteer)
    {
        var coordinates = await Tools.GetCoordinates(volunteer.Address ?? string.Empty);
        volunteer.Latitude = coordinates.Latitude;
        volunteer.Longitude = coordinates.Longitude;
        lock (AdminManager.BlMutex)
            s_dal.Volunteer.Update(ConvertFromBoToDo(volunteer));
        Observers.NotifyListUpdated();
        Observers.NotifyItemUpdated(volunteer.Id);
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
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private static bool IsValidID(int id)
    {if (id == 0)

            throw new BO.BlNullPropertyException("Null property");
        string idStr = id.ToString("D9"); //// pad with zeros if needed to make it 9 digits
        int sum = 0;

        // Iterate over each digit of the ID
        for (int i = 0; i < 9; i++)
        {
            int digit = int.Parse(idStr[i].ToString());

            // If the position is odd (1-based index), multiply by 2
            if (i % 2 == 1)
            {
                digit *= 2;
                // If the result is greater than 9, subtract 9
                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
        }

        // If the sum is divisible by 10, the ID is valid
        return sum % 10 == 0;
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
    private static bool IsEncrypted(string input)
    {
        try
        {
            Convert.FromBase64String(input);
            return true;
        }
        catch
        {
            return false;
        }
    }



    // / <summary>
    /// Converts a DO.Volunteer to a BO.Volunteer.
    /// </summary>
    /// <param name = "myVolunteer" > The DO.Volunteer object to convert.</param>
    /// <returns>Returns the converted BO.Volunteer object.</returns>
    public static BO.Volunteer ConvertFromDoToBo(DO.Volunteer myVolunteer)
    {
        IEnumerable<DO.Assignment>? assignments;
        int totalCallsHandled, totalCallsCancelled, totalCallsExpired;
        lock (AdminManager.BlMutex)
        {
            assignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == myVolunteer.Id).ToList();
            totalCallsHandled = s_dal.Assignment.ReadAll(a => a.VolunteerId == myVolunteer.Id && a.FinishType == DO.MyFinishType.Treated).Count();
            totalCallsCancelled = s_dal.Assignment.ReadAll(a => a.VolunteerId == myVolunteer.Id && (a.FinishType == DO.MyFinishType.SelfCancel || a.FinishType == DO.MyFinishType.ManagerCancel)).Count();
            totalCallsExpired = s_dal.Assignment.ReadAll(a => a.VolunteerId == myVolunteer.Id && a.FinishType == DO.MyFinishType.ExpiredCancel).Count();
        }
        return new BO.Volunteer
        {
            Id = myVolunteer.Id,
            FullName = myVolunteer.FullName,
            Phone = myVolunteer.Phone,
            Email = myVolunteer.Email,
            Password = VolunteerManager.Decrypt(myVolunteer.Password ?? string.Empty),
            Address = myVolunteer.Address,
            Latitude = myVolunteer.Latitude,
            Longitude = myVolunteer.Longitude,
            Role = (BO.MyRole)myVolunteer.Role,
            IsActive = myVolunteer.IsActive,
            MaxDistance = myVolunteer.MaxDistance,
            TypeDistance = (BO.MyTypeDistance)myVolunteer.TypeDistance,
            TotalCallsHandled = totalCallsHandled,
            TotalCallsCancelled = totalCallsCancelled,
            TotalCallsExpired = totalCallsExpired,
            CurrentCall = assignments
    .Where(a => a.FinishType == null)
    .Select(a =>
    {
        var callData = GetCall(a.CallId);
        return new BO.CallInProgress
        {
            Id = a.Id,
            CallId = a.CallId,
            CallType = (BO.MyCallType)callData.CallType,
            Description = callData.Description,
            Address = callData.Address,
            StartTime = callData.OpenTime,
            MaxEndTime = callData.MaxFinishCall,
            StartTreatmentTime = callData.OpenTime,
            DistanceFromVolunteer = Tools.GlobalDistance(myVolunteer.Address ?? string.Empty, callData.Address, myVolunteer.TypeDistance),
            Status = VolunteerManager.DetermineCallStatus(callData.MaxFinishCall)
        };
    })
    .FirstOrDefault()
        };
    }
    private static DO.Call? GetCall(int id)
    {
        lock (AdminManager.BlMutex)
            return s_dal.Call.Read(id);
    }

    /// <summary>
    /// Converts a DO.Volunteer to a BO.VolunteerInList.
    /// </summary>
    /// <param name="VolunteerData">The DO.Volunteer object to convert.</param>
    /// <returns>Returns the converted BO.VolunteerInList object.</returns>
    public static BO.VolunteerInList ConvertToVolunteerInList(DO.Volunteer VolunteerData)
    {
        int totalCallsHandled, totalCallsCancelled, totalCallsExpired;
        int? currentCallId;
        lock (AdminManager.BlMutex)
        {
            totalCallsHandled = s_dal.Assignment.ReadAll(a => a.VolunteerId == VolunteerData.Id && a.FinishType == DO.MyFinishType.Treated).Count();
            totalCallsCancelled = s_dal.Assignment.ReadAll(a => a.VolunteerId == VolunteerData.Id && (a.FinishType == DO.MyFinishType.SelfCancel || a.FinishType == DO.MyFinishType.ManagerCancel)).Count();
            totalCallsExpired = s_dal.Assignment.ReadAll(a => a.VolunteerId == VolunteerData.Id && a.FinishType == DO.MyFinishType.ExpiredCancel).Count();
            currentCallId = s_dal.Assignment.ReadAll(a => a.VolunteerId == VolunteerData.Id && a.FinishCall == null).Select(a => (int?)a.CallId).FirstOrDefault();
        }
        return new BO.VolunteerInList
        {
            Id = VolunteerData.Id,
            FullName = VolunteerData.FullName,
            IsActive = VolunteerData.IsActive,
            TotalCallsHandled = totalCallsHandled,
            TotalCallsCancelled = totalCallsCancelled,
            TotalCallsExpired = totalCallsExpired,
            CurrentCallId = currentCallId,
            CurrentCallType = (BO.MyCallType)GetCallType(VolunteerData.Id)
        };
    }


    private static DO.MyCallType GetCallType(int id)
    {
        IEnumerable<DO.Assignment> assignment;
        DO.Call? call;
        lock (AdminManager.BlMutex)
            assignment = s_dal.Assignment.ReadAll();
        DO.Assignment? ass = assignment.FirstOrDefault(a => a.VolunteerId == id && a.FinishType == null);
        if (ass != null)
        {
            lock (AdminManager.BlMutex)
                call = s_dal.Call.Read(ass.CallId);
            if (call != null)
            {
                return call.CallType;
            }
        }
        // If no assignment is found for the given volunteer ID, return MyCallType.None.
        return DO.MyCallType.None;
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
    public static async Task updateCoordinatesForVolunteerAddressAsync(DO.Volunteer volunteer)
    {
        if (volunteer.Address is not null)
        {
            (double latitude,double longitude)= await Tools.GetCoordinates(volunteer.Address);
            volunteer = volunteer with { Latitude = latitude, Longitude = longitude };
                lock (AdminManager.BlMutex)
                    s_dal.Volunteer.Update(volunteer);
                Observers.NotifyListUpdated();
                Observers.NotifyItemUpdated(volunteer.Id);
        }
    }
    /// <summary>
    /// A synchronous "volunteer activity" simulation method that operates asynchronously
    /// </summary>
    /// <exception cref="BO.BLDoesNotExistException"></exception>
    internal static void SimulateVolunteer()
    {
        List<DO.Volunteer> activeVolunteersList;
        lock (AdminManager.BlMutex)
            activeVolunteersList = s_dal.Volunteer.ReadAll(v => v.IsActive).ToList(); // List of active volunteers

        foreach (var vol in activeVolunteersList)
        {
            Assignment? assignmentInProgressData;
            lock (AdminManager.BlMutex)
                assignmentInProgressData = s_dal.Assignment.Read(a => a.VolunteerId == vol.Id && a.FinishType == null);

            if (assignmentInProgressData == null)
            {
                int percent = s_rand.Next(1, 1); // 20% chance to choose a call
                if (percent == 1)
                {
                    DO.Call? openCall;
                    lock (AdminManager.BlMutex)
                    {
                        openCall = (
                            from call in s_dal.Call.ReadAll()
                            where CallManager.GetCallStatus(call) == BO.MyCallStatus.Open || CallManager.GetCallStatus(call) == BO.MyCallStatus.OpenAtRisk
                            let distance = vol.Latitude != null && vol.Longitude != null
                                ? Tools.GlobalDistance(vol.Address,call.Address, vol.TypeDistance)
                                : double.MaxValue
                            where distance <= vol.MaxDistance
                            select call
                        ).FirstOrDefault();
                    }

                    if (openCall != null)
                    {
                        var newAssignment = new DO.Assignment
                        {
                            VolunteerId = vol.Id,
                            CallId = openCall.Id,
                            StartCall = AdminManager.Now
                        };

                        lock (AdminManager.BlMutex)
                            s_dal.Assignment.Create(newAssignment);

                        CallManager.Observers.NotifyItemUpdated(newAssignment.Id);
                        CallManager.Observers.NotifyListUpdated();
                    }
                }
            }
            else
            {
                double enoughTime;
                DO.Call? call;
                DO.Assignment assignment;
                lock (AdminManager.BlMutex)
                {
                     call = s_dal.Call.Read(assignmentInProgressData.CallId);
                    assignment=s_dal.Assignment.ReadAll(a=>a.CallId==call.Id&&a.FinishType==null).Last();
                    enoughTime = Tools.GlobalDistance(vol.Address, call.Address, vol.TypeDistance) * 4 + 30;
                }
                if (assignment != null)
                {
                    if (AdminManager.Now >= assignmentInProgressData.StartCall.AddMinutes(enoughTime))
                    {
                        var updatedAssignment = assignmentInProgressData with
                        {
                            FinishType = DO.MyFinishType.Treated,
                            FinishCall = AdminManager.Now
                        };

                        lock (AdminManager.BlMutex)
                            s_dal.Assignment.Update(updatedAssignment);

                        CallManager.Observers.NotifyItemUpdated(updatedAssignment.Id);
                        CallManager.Observers.NotifyListUpdated();
                    }
                    else
                    {
                        int percent = s_rand.Next(1, 1); // 10% chance to cancel the assignment
                        if (percent == 1)
                        {
                            var updatedAssignment = assignmentInProgressData with
                            {
                                FinishType = DO.MyFinishType.SelfCancel,
                                FinishCall = AdminManager.Now
                            };

                            lock (AdminManager.BlMutex)
                                s_dal.Assignment.Update(updatedAssignment);

                            CallManager.Observers.NotifyItemUpdated(updatedAssignment.Id);
                            CallManager.Observers.NotifyListUpdated();
                        }
                    }
                }
            }
        }
    }


    //internal static void SimulateVolunteer()
    //{
    //    List<DO.Volunteer> activeVolunteersList;
    //    lock (AdminManager.BlMutex)
    //        activeVolunteersList = s_dal.Volunteer.ReadAll(v => v.IsActive == true).ToList(); // List of active volunteers
    //    foreach (var vol in activeVolunteersList)
    //    {
    //        Assignment? assignmentInProgressData;
    //        lock (AdminManager.BlMutex)
    //            assignmentInProgressData = s_dal.Assignment.Read(a => a.VolunteerId == vol.Id && a.FinishType == null);
    //        if (assignmentInProgressData == null)
    //        {
    //            int percent = s_rand.Next(1, 2);
    //            if (percent == 1) // the probability of 20 percent to choose call
    //            {
    //                DO.Call? openCall;
    //                lock (AdminManager.BlMutex)
    //                {
    //                    openCall = (
    //                        from assignment in s_dal.Assignment.ReadAll() // Get all assignments
    //                        let call = s_dal.Call.Read(assignment.CallId) // Retrieve the call associated with the assignment
    //                        ?? throw new BO.BlDoesNotExistException($"Call with ID={assignment.CallId} does not exist") // Throw an exception if the call does not exist
    //                        where CallManager.GetCallStatus(call) == BO.MyCallStatus.Open || CallManager.GetCallStatus(call) == BO.MyCallStatus.OpenAtRisk // Check if the call is open or at risk
    //                        let distance = vol.Latitude != null && vol.Longitude != null // Calculate the distance between the volunteer and the call address
    //                            ? Tools.GlobalDistance(
    //                                vol.Address,
    //                                call.Address,
    //                                (DO.MyTypeDistance)vol.TypeDistance
    //                            )
    //                            : double.MaxValue
    //                        where distance <= vol.MaxDistance // Check if the volunteer is within the maximum distance
    //                        select call
    //                    ).ToList().FirstOrDefault();
    //                }
    //            }
    //        }
    //        else
    //        {
    //            double enoughTime;
    //            lock (AdminManager.BlMutex) 
    //            {
    //                enoughTime = Tools.GlobalDistance(
    //               vol.Address,
    //                    s_dal.Call.Read(assignmentInProgressData.CallId).Address,
    //                    (DO.MyTypeDistance)vol.TypeDistance);
    //                enoughTime = enoughTime * 2 + 10; // our decision is two minutes per kilometer and ten minutes for treatment
    //            }
    //            if (AdminManager.Now >= assignmentInProgressData.StartCall.AddMinutes(enoughTime)) // checking if enough time over from the beginning of the call treatment
    //            {
    //                // Create an updated assignment with the end of treatment details
    //                DO.Assignment updatedAssignment = assignmentInProgressData with
    //                {
    //                    FinishType = DO.MyFinishType.Treated, // Mark the assignment as treated.
    //                    FinishCall = AdminManager.Now // Set the end time to the current system time.
    //                };

    //                lock (AdminManager.BlMutex)
    //                {
    //                    // Update the assignment in the data layer.
    //                    s_dal.Assignment.Update(updatedAssignment);
    //                }
    //                // Report to the screen about a change in the BL Layer
    //                CallManager.Observers.NotifyItemUpdated(updatedAssignment.Id);
    //                CallManager.Observers.NotifyListUpdated();
    //            }
    //            else
    //            {
    //                int percent = s_rand.Next(1, 10);
    //                if (percent == 1) // the probability of 10 percent to cancel an assignment
    //                {
    //                    // Create a new assignment entity with updated cancellation details
    //                    DO.Assignment updatedAssignment = assignmentInProgressData with
    //                    {
    //                        FinishType = DO.MyFinishType.SelfCancel,
    //                        FinishCall = AdminManager.Now // End time set to the current system time.
    //                    };

    //                    lock (AdminManager.BlMutex) // stage 7
    //                    {
    //                        // Update the assignment in the data layer.
    //                        s_dal.Assignment.Update(updatedAssignment);
    //                    }
    //                    // Report to the screen about a change in the BL Layer
    //                    CallManager.Observers.NotifyItemUpdated(updatedAssignment.Id);
    //                    CallManager.Observers.NotifyListUpdated();
    //                }
    //            }
    //        }

    //    }
    //}
}

﻿using BlApi;
using BO;
using DalApi;
using DO;
using System.Net;
using System.Net.Mail;
using System.Runtime.Intrinsics.Arm;
namespace Helpers;
internal static class CallManager
{
    internal static ObserverManager Observers = new(); 

    private static IDal s_dal = DalApi.Factory.Get;
    /// <summary>
    /// Converts an instance of DO.Call to an instance of BO.Call.
    /// This includes converting the related assignments to BO.CallAssignInList objects.
    /// </summary>
    /// <param name="callData">The DO.Call instance to convert.</param>
    /// <returns>A new BO.Call instance with the converted data.</returns>

    public static BO.Call ConvertFromDoToBo(DO.Call callData)
    {
        IEnumerable<DO.Assignment> assignmentsData;
        lock (AdminManager.BlMutex)
        assignmentsData = s_dal.Assignment.ReadAll(a => a.CallId == callData.Id).ToList();
        return new BO.Call
        (
            id: callData.Id,
            type:(BO.MyCallType)(callData.CallType),
            address: callData.Address,
            latitude: callData.Latitude,
            longitude: callData.Longitude,
            startTime: callData.OpenTime,
            maxEndTime: callData.MaxFinishCall,
            description: callData.Description,
            status: GetCallStatus(callData),
            assignments: assignmentsData.Select(a => new BO.CallAssignInList
            {

                VolunteerId = a.VolunteerId != 0 ? a.VolunteerId : (int?)null,
                VolunteerName = a.VolunteerId != 0 ? GetVolunteerFullName(a.VolunteerId): null,
                StartTreatmentTime = a.StartCall,
                EndTreatmentTime = a.FinishCall,
                EndType = a.FinishType != null ? (BO.MyFinishType)a.FinishType : (BO.MyFinishType?)null
            }).ToList()
        );
    }

    private static string? GetVolunteerFullName(int volunteerId)
    {
        lock (AdminManager.BlMutex)
        {
            return s_dal.Volunteer.Read(volunteerId)?.FullName;
        }
    }


    /// <summary>
    /// Validates the format of the call values.
    /// Checks if the address is valid and not empty.
    /// </summary>
    /// <param name="call">The call object to validate.</param>
    public static void ValidateCallFormat(BO.Call call)
    {
        if (string.IsNullOrWhiteSpace(call.Address))
            throw new BO.BlNullPropertyException("Address cannot be empty.");
    }

    /// <summary>
    /// Validates the logical correctness of the call values.
    /// Ensures that the maximum end time is greater than the start time.
    /// Updates the latitude and longitude based on the address.
    /// </summary>
    /// <param name="call">The call object to validate.</param>
    public static void ValidateCallLogic(BO.Call call)
    {
        if ((call.MaxEndTime.HasValue && !IsValidDate(call.MaxEndTime.Value)))
            throw new BO.BlInvalidOperationException("Invalid date.");
        if (call.MaxEndTime.HasValue && call.MaxEndTime <= call.StartTime)
            throw new BO.BlInvalidOperationException("Max end time must be later than start time.");
        if (call.Id < 0)
            throw new BO.BlInvalidOperationException("Invalid callId.");
        if (!Tools.IsValidAddress(call.Address))
            throw new BO.BlInvalidOperationException("Invalid Address.");
    }

    /// <summary>
    /// Checks if the provided date is valid.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if the date is valid, false otherwise.</returns>
    private static bool IsValidDate(DateTime date)
    {
        return date > DateTime.MinValue && date < DateTime.MaxValue;
    }

    /// <summary>
    /// Determines the status of a call based on its assignments and maximum finish time.
    /// </summary>
    /// <param name="callD">The DO.Call instance to evaluate.</param>
    /// <returns>The status of the call as a BO.MyCallStatus enum value.</returns>
    internal static BO.MyCallStatus GetCallStatus(DO.Call callD)
    {
        IEnumerable<DO.Assignment> assignmentsList;
        DO.Call? call;
        lock (AdminManager.BlMutex)
        {
            assignmentsList = s_dal.Assignment.ReadAll(a => a.CallId == callD.Id);
            call = s_dal.Call.Read(callD.Id);
        }
        bool isInRisk = call?.MaxFinishCall - AdminManager.Now < s_dal.Config.RiskRange;
        if (assignmentsList.Any(a => a.FinishType == DO.MyFinishType.Treated))
            return BO.MyCallStatus.Closed;
        if (call?.MaxFinishCall < AdminManager.Now)
            return BO.MyCallStatus.Expired;
        if (!assignmentsList.Any())
            return isInRisk ? BO.MyCallStatus.OpenAtRisk : BO.MyCallStatus.Open;
        if (assignmentsList.Any(a => a.FinishType == null && a.FinishType == null))
            return isInRisk ? BO.MyCallStatus.InProgressAtRisk : BO.MyCallStatus.InProgress;
        return isInRisk ? BO.MyCallStatus.OpenAtRisk : BO.MyCallStatus.Open;
    }


    /// <summary>
    /// Sorts the list of calls based on the specified sorting criteria.
    /// </summary>
    /// <param name="calls">The list of calls to sort.</param>
    /// <param name="sortBy">The sorting criteria.</param>
    /// <returns>The sorted list of calls.</returns>
    public static IEnumerable<BO.CallInList> SortCalls(IEnumerable<BO.CallInList> calls, BO.MySortInCallInList sortBy)
    {
        return sortBy switch
        {
            BO.MySortInCallInList.Type => calls.OrderBy(call => call.Type),
            BO.MySortInCallInList.StartTime => calls.OrderBy(call => call.StartTime),
            BO.MySortInCallInList.TimeRemaining => calls.OrderBy(call => call.TimeRemaining),
            BO.MySortInCallInList.CompletionTime => calls.OrderBy(call => call.CompletionTime),
            BO.MySortInCallInList.Status => calls.OrderBy(call => call.Status),
            BO.MySortInCallInList.TotalAssignments => calls.OrderBy(call => call.TotalAssignments),
            _ => calls.OrderBy(call => call.CallId)
        };
    }
    /// <summary>
    /// Retrieves the value of the specified field from a BO.CallInList object.
    /// </summary>
    /// <param name="call">The BO.CallInList object.</param>
    /// <param name="field">The field to retrieve.</param>
    /// <returns>The value of the specified field.</returns>
    public static object? GetFieldValue(BO.CallInList call, BO.MySortInCallInList field)
    {
        return field switch
        {
            BO.MySortInCallInList.Type => call.Type,
            BO.MySortInCallInList.StartTime => call.StartTime,
            BO.MySortInCallInList.TimeRemaining => call.TimeRemaining,
            BO.MySortInCallInList.CompletionTime => call.CompletionTime,
            BO.MySortInCallInList.Status => call.Status,
            BO.MySortInCallInList.TotalAssignments => call.TotalAssignments,
            _ => null
        };
    }

    /// <summary>
    /// Converts an instance of DO.Call to an instance of BO.CallInList.
    /// </summary>
    /// <param name="callData">The DO.Call instance to convert.</param>
    /// <returns>A new BO.CallInList instance with the converted data.</returns>
    public static BO.CallInList ConvertToCallInList(DO.Call callData)
    {
        IEnumerable<DO.Assignment> assignmentsData;
        string? lastVolunteerName = null;
        lock (AdminManager.BlMutex)
            assignmentsData = s_dal.Assignment.ReadAll(a => a.CallId == callData.Id).ToList();
        var lastAssignment = assignmentsData.OrderByDescending(a => a.StartCall).FirstOrDefault();
        if (lastAssignment != null)
        {
            lock (AdminManager.BlMutex)
                lastVolunteerName = s_dal.Volunteer.Read(lastAssignment.VolunteerId)?.FullName;
        }
        return new BO.CallInList
        {
            Id = lastAssignment?.Id ?? 0,
            CallId = callData.Id,
            Type = (BO.MyCallType)callData.CallType,
            StartTime = callData.OpenTime,
            TimeRemaining = callData.MaxFinishCall.HasValue
                ? (callData.MaxFinishCall.Value - AdminManager.Now < TimeSpan.Zero ? TimeSpan.Zero : callData.MaxFinishCall.Value - AdminManager.Now)
                : (TimeSpan?)null,
            LastVolunteerName = lastAssignment != null && lastAssignment.VolunteerId != 0 ? lastVolunteerName : null,
            CompletionTime = lastAssignment?.FinishCall.HasValue == true ? lastAssignment.FinishCall.Value - callData.OpenTime : (TimeSpan?)null,
            Status = GetCallStatus(callData),
            TotalAssignments = assignmentsData.Count()
        };
    }



    /// <summary>
    /// Sends an email notification to the specified email address.
    /// </summary>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body of the email.</param>
    public static async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var fromEmail = "y7697086@gmail.com";
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("y7697086@gmail.com", "zrth ljnd mujf muxt"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail),
            Subject = subject,
            Body =body,
            IsBodyHtml = false,
        };
        mailMessage.To.Add(toEmail);
        await smtpClient.SendMailAsync(mailMessage);
    }

    /// <summary>
    /// Converts an instance of BO.Call to an instance of DO.Call.
    /// </summary>
    /// <param name="myCall">The BO.Call instance to convert.</param>
    /// <returns>A new DO.Call instance with the converted data.</returns>
    public static DO.Call ConvertBOToDO(BO.Call myCall)
    {
        return new DO.Call
        {
            Id = myCall.Id,
            CallType = (DO.MyCallType)myCall.Type,
            Address = myCall.Address ?? "",
            Latitude = myCall.Latitude,
            Longitude = myCall.Longitude,
            OpenTime = myCall.StartTime,
            Description = myCall.Description,
            MaxFinishCall = myCall.MaxEndTime
        };
    }

    /// <summary>
    /// Sorts a list of closed calls by the specified field.
    /// </summary>
    /// <param name="calls">The list of closed calls to sort.</param>
    /// <param name="sortBy">The field to sort by.</param>
    /// <returns>The sorted list of closed calls.</returns>
    public static IEnumerable<BO.ClosedCallInList> SortClosedCallsByField(IEnumerable<BO.ClosedCallInList> calls, BO.CloseCall? sortBy)
    {
        return from call in calls
               orderby sortBy switch
               {
                   BO.CloseCall.Address => call.Address as object,
                   BO.CloseCall.StartTime => call.StartTime as object,
                   BO.CloseCall.StartTreatmentTime => call.StartTreatmentTime as object,
                   BO.CloseCall.EndTime => call.EndTime as object,
                   BO.CloseCall.EndType => call.EndType as object,
                   _ => call.Id as object
               }
               select call;
    }

    /// <summary>
    /// Sorts a list of open calls by the specified field.
    /// </summary>
    /// <param name="openCallInList">The list of open calls to sort.</param>
    /// <param name="sortBy">The field to sort by.</param>
    /// <returns>The sorted list of open calls.</returns>
    public static IEnumerable<BO.OpenCallInList> SortOpenCallsByField(IEnumerable<BO.OpenCallInList> openCallInList, BO.OpenedCall? sortBy)
    {
        return sortBy switch
        {
            BO.OpenedCall.Type => openCallInList.OrderBy(call => call.Type),
            BO.OpenedCall.Address => openCallInList.OrderBy(call => call.Address),
            BO.OpenedCall.StartTime => openCallInList.OrderBy(call => call.StartTime),
            BO.OpenedCall.MaxEndTime => openCallInList.OrderBy(call => call.MaxEndTime),
            BO.OpenedCall.DistanceFromVolunteer => openCallInList.OrderBy(call => call.DistanceFromVolunteer),
            _ => openCallInList.OrderBy(call => call.Id)
        };
    }

    /// <summary>
    /// Converts an instance of DO.Assignment to an instance of BO.ClosedCallInList.
    /// </summary>
    /// <param name="assignment">The DO.Assignment instance to convert.</param>
    /// <returns>A new BO.ClosedCallInList instance with the converted data.</returns>
    public static BO.ClosedCallInList convertAssignmentToClosed(DO.Assignment assignment)
    {
        DO.Call? callDetails;
        lock (AdminManager.BlMutex)
            callDetails = s_dal.Call.Read(assignment.CallId);
        if (callDetails == null)
            throw new BO.BlNullPropertyException("Call details cannot be null.");
        return new BO.ClosedCallInList
        {
            Id = assignment.CallId,
            Type = (BO.MyCallType)callDetails.CallType,
            Address = callDetails.Address,
            StartTime = callDetails.OpenTime,
            StartTreatmentTime = assignment.StartCall,
            EndTime = assignment.FinishCall,
            EndType = assignment?.FinishType is null ? (BO.MyFinishType?)null : (BO.MyFinishType)assignment.FinishType
        };
    }

    /// <summary>
    /// Determines if a call should be considered open based on its status.
    /// </summary>
    /// <param name="call">The call to check.</param>
    /// <returns>True if the call is open, false otherwise.</returns>
    public static bool OpenCondition(DO.Call call)
    {
        // Convert DO.Call to BO.Call
        var newCall = call != null ? ConvertFromDoToBo(call) : null;
        // Check if the status of the call is Open or OpenAtRisk
        return (newCall?.Status == BO.MyCallStatus.Open || newCall?.Status == BO.MyCallStatus.OpenAtRisk) == true;
    }
    /// <summary>
    /// Determines if the call is within the volunteer's max distance.
    /// </summary>
    /// <param name="volunteer">The volunteer to check.</param>
    /// <param name="call">The call to check.</param>
    /// <returns>True if the call is within the max distance, false otherwise.</returns>
    public static bool VolunteerArea(DO.Volunteer volunteer, DO.Call call)
    {
        // Calculate the distance between the volunteer's address and the call's address
        if (volunteer.Address == null || call.Address == null)
        {
            return false;
        }
        double distance = Tools.GlobalDistance(volunteer.Address, call.Address, volunteer.TypeDistance);
        // Check if the distance is within the volunteer's max distance
        return distance <= volunteer.MaxDistance;
    }

    /// <summary>
    /// Converts an instance of DO.Call to an instance of BO.OpenCallInList.
    /// </summary>
    /// <param name="volunteer">The volunteer handling the call.</param>
    /// <param name="callDetails">The details of the call.</param>
    /// <returns>A new BO.OpenCallInList instance with the converted data.</returns>
    public static BO.OpenCallInList convertCallToOpened(DO.Volunteer volunteer, DO.Call callDetails)
    {
        return new BO.OpenCallInList
        {
            Id = callDetails.Id,
            Type = (BO.MyCallType)callDetails.CallType,
            Description = callDetails.Description,
            Address = callDetails.Address,
            StartTime = callDetails.OpenTime,
            MaxEndTime = callDetails.MaxFinishCall,
            DistanceFromVolunteer = volunteer.Address != null && callDetails.Address != null
                ? Tools.GlobalDistance(volunteer.Address, callDetails.Address, volunteer.TypeDistance)
                : 0,
        };
    }

    /// <summary>
    /// Checks if a volunteer with the given ID is a manager.
    /// </summary>
    /// <param name="idV">The ID of the volunteer.</param>
    /// <returns>True if the volunteer is a manager, false otherwise.</returns>
    public static bool IsManager(int idV)
    {
        return s_dal.Volunteer.Read(idV)?.Role == DO.MyRole.Manager;
    }

    /// <summary>
    /// Determines if any assignment in the collection is open based on its status.
    /// </summary>
    /// <param name="assignments">The collection of assignments to check.</param>
    /// <returns>True if any assignment is open, false otherwise.</returns>
    public static bool IsOpenAssignment(IEnumerable<DO.Assignment> assignments)
    {
        return assignments.Any(a => !a.FinishCall.HasValue && !a.FinishType.HasValue);
    }
    public static async Task updateCoordinatesForCallAddressAsync(DO.Call call)
    {
        if (call.Address is not null)
        {
            (double latitude, double longitude) = await Tools.GetCoordinates(call.Address);
            call = call with { Latitude = latitude, Longitude = longitude };
            lock (AdminManager.BlMutex)
                s_dal.Call.Update(call);
            Observers.NotifyListUpdated();
            Observers.NotifyItemUpdated(call.Id);
        }
    }
    /// <summary>
    /// Sends an email notification to volunteers when a new call is opened.
    /// </summary>
    /// <param name="call">The call details to include in the email.</param>
    public static async Task AddCallSendEmailAsync(DO.Call call)
    {
        var volunteers = s_dal.Volunteer.ReadAll(volunteer =>
            volunteer.Address != null &&
            volunteer.MaxDistance >= Tools.GlobalDistance(volunteer.Address, call.Address, volunteer.TypeDistance));
        foreach (var volunteer in volunteers)
        {
            var subject = $"New Call Opened for {call.Id}";
            var body = $"Hello Volunteers,\n\n" +  // General greeting
                       $"A new call has been opened. Here are the details:\n\n" +
                       $"Description: {call.Description}\n" +
                       $"Location: {call.Address}\n" +
                       $"Please log in to the system to accept the call."; // Body of the email;
            await CallManager.SendEmailAsync(volunteer.Email, subject, body);
        }
    }
    /// <summary>
    /// Updates the status of a call and sends an email notification when a treatment is canceled.
    /// </summary>
    /// <param name="idV">The ID of the volunteer.</param>
    /// <param name="assignment">The assignment details.</param>
    public static async Task UpdateCancelTreatmentSendEmailAsync(int idV, DO.Assignment assignment)
    {
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex)
            volunteer = s_dal.Volunteer.Read(assignment.VolunteerId);

        if (CallManager.IsManager(idV))
        {
            var subject = $"Assignment Cancelled";
            var body = $"Your assignment for call {assignment.CallId} has been cancelled.";
            await CallManager.SendEmailAsync(volunteer?.Email ?? string.Empty, subject, body);
        }
    }
    /// <summary>
    /// Adds coordinates to a call based on its address.
    /// </summary>
    /// <param name="call">The call object to update with coordinates.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task AddCallCoordinatesAsync(DO.Call call)
    {
        if (call.Address is not null)
        {
            var coordinates = await Tools.GetCoordinates(call.Address ?? string.Empty);
            call = call with { Latitude = coordinates.Latitude, Longitude = coordinates.Longitude };
            lock (AdminManager.BlMutex)
                s_dal.Call.Update(call);
            Observers.NotifyListUpdated();
            Observers.NotifyItemUpdated(call.Id);
        }
    }
    /// <summary>
    /// Periodically updates the status of calls that have exceeded their maximum finish time.
    /// </summary>
    internal static void PeriodicCallsUpdates()
    {
        List<int> updatedCallIds = new List<int>(); 
        IEnumerable<DO.Call> callsList;
        IEnumerable<DO.Assignment> assignmentList;
        lock (AdminManager.BlMutex)
        callsList = s_dal.Call.ReadAll(call => call.MaxFinishCall < AdminManager.Now);
        foreach (var call in callsList)
        {
            lock (AdminManager.BlMutex)
                assignmentList = s_dal.Assignment.ReadAll(a => a.CallId == call.Id);

            if (!assignmentList.Any())
            {
                lock (AdminManager.BlMutex)
                {
                    s_dal.Assignment.Create(new DO.Assignment
                    {
                        CallId = call.Id,
                        StartCall = AdminManager.Now,
                        FinishCall = AdminManager.Now,
                        FinishType = DO.MyFinishType.ExpiredCancel,
                        VolunteerId = 0
                    });
                }
                updatedCallIds.Add(call.Id);
            }

            DO.Assignment? assignmentInProgress = assignmentList.FirstOrDefault(a => a.FinishCall == null && a.FinishType == null);

            if (assignmentInProgress != null)
            {
                lock (AdminManager.BlMutex)
                {
                    s_dal.Assignment.Update(new DO.Assignment
                    {
                        Id = assignmentInProgress.Id,
                        CallId = assignmentInProgress.CallId,
                        VolunteerId = assignmentInProgress.VolunteerId,
                        StartCall = assignmentInProgress.StartCall,
                        FinishCall = AdminManager.Now,
                        FinishType = DO.MyFinishType.ExpiredCancel
                    });
                }
                updatedCallIds.Add(assignmentInProgress.Id);

            }
        }
        foreach (var callId in updatedCallIds)
        {
            Observers.NotifyItemUpdated(callId);
        }
        Observers.NotifyListUpdated(); // Add call to NotifyListUpdated
}


}
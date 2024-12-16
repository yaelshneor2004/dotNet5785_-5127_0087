
using BO;
using DalApi;
using Newtonsoft.Json;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get;
    public static BO.Call ConvertFromDoToBo(DO.Call callData)
    {
        var assignmentsData = s_dal.Assignment.ReadAll(a => a.CallId == callData.Id).ToList();

        return new BO.Call
        (
            id: callData.Id,
            type: (BO.MyCallType)callData.CallType,
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
                VolunteerName = a.VolunteerId != 0 ? s_dal.Volunteer.Read(a.VolunteerId)?.FullName : null,
                EndTreatmentTime = a.FinishCall,
                EndType = a.FinishType != null ? (BO.MyFinishType)a.FinishType : (BO.MyFinishType?)null
            }).ToList()
        );
    }

    public static BO.MyCallStatus CalculateCallStatus(DO.Assignment lastAssignment, DateTime? maxEndTime)
    {
        var currentTime = ClockManager.Now;
        var isInRiskTimeRange = maxEndTime != null && (maxEndTime.Value - currentTime) <= s_dal.Config.RiskRange;


        if (lastAssignment == null)
        {
            return maxEndTime == null || maxEndTime > currentTime
                ? BO.MyCallStatus.Open
                : BO.MyCallStatus.Expired;
        }

        if (lastAssignment.FinishCall.HasValue)
        {
            return BO.MyCallStatus.Closed;
        }

        if (maxEndTime != null && maxEndTime <= currentTime)
        {
            return BO.MyCallStatus.Expired;
        }

        if (isInRiskTimeRange)
        {
            return lastAssignment.StartCall <= currentTime
                ? BO.MyCallStatus.InProgressAtRisk
                : BO.MyCallStatus.OpenAtRisk;
        }

        return BO.MyCallStatus.InProgress;
    }
    // Method to validate the format of the values
    public static void ValidateCallFormat(BO.Call call)
    {
        // Check if the address is valid
        if (string.IsNullOrWhiteSpace(call.Address))
            throw new BO.BlNullPropertyException("Address cannot be empty.");    }

    // Method to validate the logical correctness of the values
    public static void ValidateCallLogic(BO.Call call)
    {
        // Check if the maximum end time is greater than the start time
        if (call.MaxEndTime.HasValue && call.MaxEndTime <= call.StartTime)
            throw new BO.BlInvalidOperationException("Max end time must be later than start time.");
        if (!IsValidAddress(call.Address, out double latitude, out double longitude))
        {
            throw new BO.BlException("Invalid address.");
        }
        // Update the latitude and longitude based on the address
        call.Latitude = latitude;
        call.Longitude = longitude;
        if(call.Id<0)
            throw new BO.BlInvalidOperationException("invalide callId.");

    }

    // Helper method to get latitude and longitude based on address
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

    public static BO.MyCallStatus GetCallStatus(DO.Call call)
    {
        var assignment = s_dal.Assignment.Read(call.Id);
        var now = ClockManager.Now;
        // Check if the call is in progress (if the assignment is active and has no finish time)
        if (assignment != null && assignment.FinishCall == null)
        {
            // Check if the call is in progress at risk
            if (call.MaxFinishCall.HasValue && now > call.MaxFinishCall.Value - s_dal.Config.RiskRange)
                return BO.MyCallStatus.InProgressAtRisk;
            return BO.MyCallStatus.InProgress;
        }
        // Check if the call is expired
        // Condition 1: The call has a max finish time and the current time is past that time
        // Condition 2: The call has no active assignment and the current time is past the max finish time
        if (call.MaxFinishCall.HasValue && now > call.MaxFinishCall.Value && (assignment == null || assignment.FinishCall == null))
            return BO.MyCallStatus.Expired;
        // Check if the call is closed (if the assignment has a finish time)
        if (assignment != null && assignment.FinishCall.HasValue)
            return BO.MyCallStatus.Closed;
        // Check if the call is open at risk
        if (call.MaxFinishCall.HasValue && now > call.OpenTime + (s_dal.Config.RiskRange))
            return BO.MyCallStatus.OpenAtRisk;
        // Default: Open call
        return BO.MyCallStatus.Open;
    }

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
    public static BO.CallInList ConvertToCallInList(DO.Call callData)
    {
            var assignmentsData = s_dal.Assignment.ReadAll(a => a.CallId == callData.Id).ToList();
        var lastAssignment = assignmentsData.OrderByDescending(a => a.StartCall).FirstOrDefault();
        return new BO.CallInList
            {
                Id = lastAssignment?.Id ?? 0,
                CallId = callData.Id,
                Type = (BO.MyCallType)callData.CallType,
                StartTime = callData.OpenTime,
                TimeRemaining = callData.MaxFinishCall.HasValue? callData.MaxFinishCall.Value - ClockManager.Now  : (TimeSpan?)null,
                LastVolunteerName = lastAssignment != null && lastAssignment.VolunteerId != 0 ? s_dal.Volunteer.Read(lastAssignment.VolunteerId)?.FullName : null,
                CompletionTime = lastAssignment?.FinishCall.HasValue == true ? lastAssignment.FinishCall.Value - callData.OpenTime : (TimeSpan?)null,
                Status = GetCallStatus(callData),
                TotalAssignments = assignmentsData.Count()
            };

    }

    public static DO.Call ConvertBOToDO(BO.Call myCall)
    {
       return new DO.Call
        {
            Id = myCall.Id,
            CallType = (DO.MyCallType)myCall.Type,
            Address = myCall.Address ?? "",
            Latitude = myCall.Latitude ?? 0,
            Longitude = myCall.Longitude ?? 0,
            OpenTime = myCall.StartTime,
            Description = myCall.Description,
            MaxFinishCall = myCall.MaxEndTime
        };   
    }
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
    
    public static BO.ClosedCallInList convertAssignmentToClosed(DO.Assignment assignment)
    {
        var callDetails = s_dal.Call.Read(assignment.CallId);

        return new BO.ClosedCallInList
        {
            Id = assignment.CallId,
            Type = (BO.MyCallType)callDetails.CallType,
            Address = callDetails.Address,
            StartTime = callDetails.OpenTime,
            StartTreatmentTime = assignment.StartCall,
            EndTime = assignment.FinishCall,
            EndType = (BO.MyFinishType)assignment.FinishType
        };
    }
   public static bool OpenCondition(DO.Assignment assignment)
    {
        var callDetails = s_dal.Call.Read(assignment.CallId);
        var call = ConvertFromDoToBo(callDetails);
        return call.Status == MyCallStatus.Open || call.Status == MyCallStatus.OpenAtRisk;
    }

    public static bool CloseCondition(DO.Assignment assignment)
    {
        var callDetails = s_dal.Call.Read(assignment.CallId);
        var call = ConvertFromDoToBo(callDetails);
        return call.Status == MyCallStatus.Expired || call.Status == MyCallStatus.Closed;
    }
    public static BO.OpenCallInList convertAssignmentToOpened(DO.Assignment assignment)
    {
        var callDetails = s_dal.Call.Read(assignment.CallId);
        var volunteer = s_dal.Volunteer.Read(assignment.VolunteerId);
        return new BO.OpenCallInList
        {
            Id = assignment.CallId,
            Type = (BO.MyCallType)callDetails.CallType,
            Description = callDetails.Description,
            Address = callDetails.Address,
            StartTime = callDetails.OpenTime,
            MaxEndTime = callDetails.MaxFinishCall,
            DistanceFromVolunteer = Tools.GlobalDistance(volunteer.Longitude, volunteer.Latitude, callDetails.Longitude, callDetails.Latitude, volunteer.TypeDistance)
        };
    }

    public static bool IsManager(int idV)
    {
       return s_dal.Volunteer.Read(idV).Role == DO.MyRole.Manager;
    }
    public static bool IsOpenAssignment(IEnumerable<DO.Assignment> assignments)
    {
        return assignments.Any(a => !a.FinishCall.HasValue && !a.FinishType.HasValue);
    }
    //public static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    //{
    //    var calls = s_dal.Call.ReadAll().ToList();

    //    for (int i = 0; i < calls.Count; i++)
    //    {
    //        var call = calls[i];

    //        אם הזמן המרבי לסגירת הקריאה עבר, מסמנים אותה כ"פגת תוקף"
    //        if (call.MaxFinishCall.HasValue && (newClock > call.MaxFinishCall))
    //        {
    //            call = call with { my = DO.MyFinishType.ExpiredCancel };
    //            s_dal.Call.Update(call); // עדכון הקריאה בבסיס הנתונים ישירות
    //        }
    //    }
    //}

    internal static void PeriodicCallsUpdates() //stage 4
    {
        var callsList = s_dal.Call.ReadAll(call => call.MaxFinishCall < ClockManager.Now);

        foreach (var call in callsList)
        {
            var assignmentList = s_dal.Assignment.ReadAll(a => a.CallId == call.Id);

            if (!assignmentList.Any())
            {
                s_dal.Assignment.Create(new DO.Assignment
                {
                    CallId = call.Id,
                    StartCall = ClockManager.Now,
                    FinishCall = ClockManager.Now,
                    FinishType = DO.MyFinishType.ExpiredCancel,
                    VolunteerId = 0
                });
            }

            DO.Assignment? assignmentInProgress = assignmentList.FirstOrDefault(a => a.FinishCall == null && a.FinishType == null);

            if (assignmentInProgress != null)
            {
                s_dal.Assignment.Update(new DO.Assignment
                {
                    Id = assignmentInProgress.Id,
                    CallId = assignmentInProgress.CallId,
                    VolunteerId = assignmentInProgress.VolunteerId,
                    StartCall = assignmentInProgress.StartCall,
                    FinishCall = ClockManager.Now,
                    FinishType = DO.MyFinishType.ExpiredCancel
                });
            }
        }
    }

}

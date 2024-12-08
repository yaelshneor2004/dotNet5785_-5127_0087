
using DalApi;
using Newtonsoft.Json;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get;
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
        {
            throw new BO.BlException("Address cannot be empty.");
        }

        // Check if the latitude and longitude are within valid ranges
        if (call.Latitude.HasValue && (call.Latitude < -90 || call.Latitude > 90))
        {
            throw new BO.BlException("Latitude must be between -90 and 90.");
        }

        if (call.Longitude.HasValue && (call.Longitude < -180 || call.Longitude > 180))
        {
            throw new BO.BlException("Longitude must be between -180 and 180.");
        }
    }

    // Method to validate the logical correctness of the values
    public static void ValidateCallLogic(BO.Call call)
    {
        // Check if the maximum end time is greater than the start time
        if (call.MaxEndTime.HasValue && call.MaxEndTime <= call.StartTime)
        {
            throw new BO.BlException("Max end time must be later than start time.");
        }
        if (!IsValidAddress(call.Address, out double latitude, out double longitude))
        {
            throw new BO.BlException("Invalid address.");
        }

        // Update the latitude and longitude based on the address
        call.Latitude = latitude;
        call.Longitude = longitude;

        // אם יש מתנדב שמוקצה לקריאה, עדכן את פרטי המתנדב
        var assignment = s_dal.Assignment.ReadAll(a => a.CallId == call.Id).FirstOrDefault();
        if (assignment != null)
        {
            //var volunteer = s_dal.Volunteer.Read(assignment.VolunteerId);
            //volunteer.Latitude= latitude;
            //volunteer.Longitude = longitude;
            //s_dal.Volunteer.Update(volunteer);
        }
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
      public static BO.MyCallStatus GetCallStatus(DO.Call call, IEnumerable<DO.Assignment> assignments)
    {
        var now = DateTime.Now;

        var activeAssignment = assignments.FirstOrDefault(a => a.CallId == call.Id && a.FinishCall == null);
        if (activeAssignment != null)
        {
            return BO.MyCallStatus.InProgress;
        }

        if (call.MaxFinishCall.HasValue && now > call.MaxFinishCall.Value)
        {
            return BO.MyCallStatus.Expired;
        }

        var finishedAssignment = assignments.FirstOrDefault(a => a.CallId == call.Id && a.FinishCall.HasValue);
        if (finishedAssignment != null)
        {
            return BO.MyCallStatus.Closed;
        }

        if (call.MaxFinishCall.HasValue && now > call.OpenTime.AddHours(1))
        {
            return BO.MyCallStatus.OpenAtRisk;
        }

        return BO.MyCallStatus.Open;
    }

}

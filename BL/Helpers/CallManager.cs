
using DalApi;
using Newtonsoft.Json;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get;
    //public static BO.MyCallStatus CalculateCallStatus(DO.Assignment lastAssignment, DateTime? maxEndTime)
    //{
    //    var currentTime = ClockManager.Now;
    //    var isInRiskTimeRange = maxEndTime != null && (maxEndTime.Value - currentTime) <= s_dal.Config.RiskRange;

    //    // הגדרת סטטוס הקריאה לפי תנאים
    //    if (lastAssignment == null)
    //    {
    //        return maxEndTime == null || maxEndTime > currentTime
    //            ? BO.MyCallStatus.Open
    //            : BO.MyCallStatus.Expired;
    //    }

    //    if (lastAssignment.FinishCall.HasValue)
    //    {
    //        return BO.MyCallStatus.Closed;
    //    }

    //    if (maxEndTime != null && maxEndTime <= currentTime)
    //    {
    //        return BO.MyCallStatus.Expired;
    //    }

    //    if (isInRiskTimeRange)
    //    {
    //        return lastAssignment.StartCall <= currentTime
    //            ? BO.MyCallStatus.InProgressAtRisk
    //            : BO.MyCallStatus.OpenAtRisk;
    //    }

    //    return BO.MyCallStatus.InProgress;
    //}

    
        public static BO.MyCallStatus CalculateCallStatus(List<DO.Assignment> assignments, DateTime? maxFinishCall)
        {
            if (assignments.Any(a => a.FinishType == DO.MyFinishType.Treated))
            {
                return BO.MyCallStatus.Closed;
            }
            else if (DateTime.Now > maxFinishCall)
            {
                return BO.MyCallStatus.Expired;
            }
            else if (assignments.Any(a => a.FinishType == DO.MyFinishType.ManagerCancel || a.FinishType == DO.MyFinishType.SelfCancel))
            {
                return BO.MyCallStatus.OpenAtRisk;
            }
            else
            {
                return BO.MyCallStatus.InProgress;
            }
        
        }

        public static void ValidateCallDetails(BO.Call call)
        {
            // בדיקה אם זמן מקסימלי לסיום גדול מזמן הפתיחה
            if (call.MaxEndTime <= call.StartTime)
            {
                throw new BO.BlException("Max end time must be greater than start time.");
            }

            // בדיקה אם הכתובת תקינה (משתמשים בפונקציה חיצונית לדוגמה)
            if (!IsValidAddress(call.Address, out double latitude, out double longitude))
            {
                throw new BO.BlException("Invalid address.");
            }

            call.Latitude = latitude;
            call.Longitude = longitude;

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
    





    //מברכה
    //    public static BO.CallStatus GetCallStatus(DO.Call call, IEnumerable<DO.Assignment> assignments)
    //{
    //    var now = DateTime.Now;

    //    var activeAssignment = assignments.FirstOrDefault(a => a.CallId == call.CallId && a.FinishTime == null);
    //    if (activeAssignment != null)
    //    {
    //        return BO.CallStatus.InCare;
    //    }

    //    if (call.MaxTime.HasValue && now > call.MaxTime.Value)
    //    {
    //        return BO.CallStatus.Expired;
    //    }

    //    var finishedAssignment = assignments.FirstOrDefault(a => a.CallId == call.CallId && a.FinishTime.HasValue);
    //    if (finishedAssignment != null)
    //    {
    //        return BO.CallStatus.Closed;
    //    }

    //    if (call.MaxTime.HasValue && now > call.OpenTime.AddHours(1))
    //    {
    //        return BO.CallStatus.OpenAtRisk;
    //    }

    //    return BO.CallStatus.Open;
    //}

}

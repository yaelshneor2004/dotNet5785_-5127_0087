
using DalApi;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get;
    public static BO.MyCallStatus CalculateCallStatus(DO.Assignment lastAssignment, DateTime? maxEndTime)
    {
        var currentTime = ClockManager.Now;
        var isInRiskTimeRange = maxEndTime != null && (maxEndTime.Value - currentTime) <=s_dal.Config.RiskRange;

        // הגדרת סטטוס הקריאה לפי תנאים
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



}

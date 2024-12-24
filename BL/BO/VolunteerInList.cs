using System.Linq;


namespace BO
{
    /// <summary>
    /// Represents the details of a volunteer.
    /// </summary>
    /// <param name="Id">Represents a standard ID that uniquely identifies the volunteer</param>
    /// <param name="FullName">Full name of the volunteer</param>
    /// <param name="IsActive">If the volunteer is active or inactive (retired from the organization)</param>
    /// <param name="TotalCallsHandled">Total number of calls handled by the volunteer</param>
    /// <param name="TotalCallsCancelled">Total number of calls canceled by the volunteer</param>
    /// <param name="TotalCallsExpired">Total number of calls that the volunteer chose to handle but expired</param>
    /// <param name="CurrentCallId">Identifier number of the call currently in progress (if any)</param>
    /// <param name="CurrentCallType">Type of the call currently in progress (ENUM)</param>

    public class VolunteerInList
    {
        public int Id { get; init; }
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int TotalCallsHandled { get; init; }
        public int TotalCallsCancelled { get; init; }
        public int TotalCallsExpired { get; init; }
        public int? CurrentCallId { get; set; }
        public BO.MyCallType CurrentCallType { get; set; }
        public override string ToString()
        {
            return $"Volunteer ID: {Id}, FullName: {FullName}, Active: {IsActive}, " + $"Total Calls Handled: {TotalCallsHandled}, Total Calls Cancelled: {TotalCallsCancelled}, " + $"Total Calls Expired: {TotalCallsExpired}, Current Call ID: {CurrentCallId}, " + $"Current Call Type: {CurrentCallType}";

        }
    }
}


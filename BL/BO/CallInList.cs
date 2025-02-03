using Helpers;

namespace BO
{
    /// <summary>
    /// Represents the details of a call in the list.
    /// </summary>
    /// <param name="Id">Identifier number of the assignment entity</param>
    /// <param name="CallId">Running identifier number of the call entity</param>
    /// <param name="Type">Type of the call (ENUM)</param>
    /// <param name="StartTime">Call opening time</param>
    /// <param name="TimeRemaining">Time remaining until the call's maximum end time (TimeSpan)</param>
    /// <param name="LastVolunteerName">Name of the last volunteer assigned to the call</param>
    /// <param name="CompletionTime">Total time taken to complete the call (TimeSpan)</param>
    /// <param name="Status">Status of the call (ENUM)</param>
    /// <param name="TotalAssignments">Total number of assignments for the call</param>
    public class CallInList
    {
        public int? Id { get; set; }
        public int CallId { get; set; }
        public MyCallType Type { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
        public string? LastVolunteerName { get; set; }
        public TimeSpan? CompletionTime { get; set; }
        public MyCallStatus Status { get; set; }
        public int TotalAssignments { get; set; }
        public override string ToString() => this.ToStringProperty();
    }

}

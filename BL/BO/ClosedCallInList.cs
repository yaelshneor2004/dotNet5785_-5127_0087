using DO;

namespace BO
{
    /// <summary>
    /// Represents the details of a closed call in the list.
    /// </summary>
    /// <param name="Id">Running identifier number of the call entity</param>
    /// <param name="Type">Type of the call (ENUM)</param>
    /// <param name="Address">Full address of the call</param>
    /// <param name="StartTime">Call opening time</param>
    /// <param name="StartTreatmentTime">Start treatment time by the volunteer</param>
    /// <param name="EndTime">Actual end time of the treatment</param>
    /// <param name="EndType">Type of the treatment end (ENUM)</param>
    public class ClosedCallInList
    {
        public int Id { get; init; }
        public MyCallType Type { get; set; }
        public string Address { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StartTreatmentTime { get; set; }
        public DateTime? EndTime { get; set; }
        public MyFinishType? EndType { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}, Type: {Type}, Address: {Address}, StartTime: {StartTime}, " +
                   $"StartTreatmentTime: {StartTreatmentTime}, EndTime: {EndTime}, EndType: {EndType}";
        }
    }

}

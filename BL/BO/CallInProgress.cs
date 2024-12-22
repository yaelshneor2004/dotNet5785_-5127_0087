namespace BO
{
    /// <summary>
    /// Details about the volunteer and his attributes.
    /// </summary>
    /// <param name="Id">Represents a standard ID that uniquely identifies the volunteer</param>
    /// <param name="CallId">Running identifier number of the call entity</param>
    /// <param name="CallType">Type of the call (ENUM)</param>
    /// <param name="Description">Literal description of the call</param>
    /// <param name="Address">Full address of the call</param>
    /// <param name="StartTime">Call opening time</param>
    /// <param name="MaxEndTime">Maximum end time of the call</param>
    /// <param name="StartTreatmentTime">Start treatment time by the volunteer</param>
    /// <param name="DistanceFromVolunteer">Distance of the call from the volunteer handling it</param>
    /// <param name="Status">Status of the call (ENUM)</param>
    public class CallInProgress
    {
        public int Id { get; init; }
        public int CallId { get; set; }
        public MyCallType CallType { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; }=string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? MaxEndTime { get; set; }
        public DateTime StartTreatmentTime { get; set; }
        public double DistanceFromVolunteer { get; set; }
        public MyCallStatusByVolunteer Status { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}, CallId: {CallId}, CallType: {CallType}, Description: {Description}, " +
                   $"Address: {Address}, StartTime: {StartTime}, MaxEndTime: {MaxEndTime}, " +
                   $"StartTreatmentTime: {StartTreatmentTime}, DistanceFromVolunteer: {DistanceFromVolunteer}, Status: {Status}";
        }
    }

}

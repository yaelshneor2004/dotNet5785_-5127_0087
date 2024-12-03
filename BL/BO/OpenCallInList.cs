namespace BO
{
    /// <summary>
    /// Represents the details of an open call in the list.
    /// </summary>
    /// <param name="Id">Running identifier number of the call entity</param>
    /// <param name="Type">Type of the call (ENUM)</param>
    /// <param name="Description">Literal description of the call</param>
    /// <param name="Address">Full address of the call</param>
    /// <param name="StartTime">Call opening time</param>
    /// <param name="MaxEndTime">Maximum end time of the call</param>
    /// <param name="DistanceFromVolunteer">Distance of the call from the volunteer handling it</param>
    public class OpenCallInList
    {
        public int Id { get; init; }
        public MyCallType Type { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? MaxEndTime { get; set; }
        public double DistanceFromVolunteer { get; set; }
    }
}

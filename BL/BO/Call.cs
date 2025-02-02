namespace BO
{
    /// <summary>
    /// Represents the details of a call.
    /// </summary>
    /// <param name="Id">Running identifier number of the call entity</param>
    /// <param name="Type">Type of the call (ENUM)</param>
    /// <param name="Description">Literal description of the call</param>
    /// <param name="Address">Full address of the call</param>
    /// <param name="Latitude">Latitude of the call location</param>
    /// <param name="Longitude">Longitude of the call location</param>
    /// <param name="StartTime">Call opening time</param>
    /// <param name="MaxEndTime">Maximum end time of the call</param>
    /// <param name="Status">Status of the call (ENUM)</param>
    /// <param name="Assignments">List of assignments for the call</param>
    public class Call
    {
        public int Id { get; init; }
        public MyCallType Type { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? MaxEndTime { get; set; }
        public MyCallStatus Status { get; set; }
        public List<BO.CallAssignInList>? Assignments { get; set; }


                public Call(int id, MyCallType type, string address, double latitude, double longitude, DateTime startTime, DateTime? maxEndTime, string? description, MyCallStatus status, List<BO.CallAssignInList>? assignments = null)
                {
                    Id = id;
                    Type = type;
                    Address = address;
                    Latitude = latitude;
                    Longitude = longitude;
                    StartTime = startTime;
                    MaxEndTime = maxEndTime;
                    Description = description;
                    Status = status;
                    Assignments = assignments ?? new List<BO.CallAssignInList>();
                }

        public Call()
        {
            Address = string.Empty;
            Latitude = 0.0;
            Longitude = 0.0;
            StartTime = DateTime.MinValue;
            Status = MyCallStatus.None;
            Type = MyCallType.None;
            Assignments = new List<BO.CallAssignInList>();
        }

        public override string ToString()
        {
            return $"Id: {Id}, Type: {Type}, Description: {Description}, Address: {Address}, " +
                   $"Latitude: {Latitude}, Longitude: {Longitude}, StartTime: {StartTime}, " +
                   $"MaxEndTime: {MaxEndTime}, Status: {Status}, " +
                   $"Assignments: {string.Join(", ", Assignments?.Select(a => a.ToString()) ?? new List<string>())}";
        }
    }
}

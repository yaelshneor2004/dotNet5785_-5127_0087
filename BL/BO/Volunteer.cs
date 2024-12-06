
namespace BO
{
        /// <summary>
        /// Details about the volunteer and his attributes.
        /// </summary>
        /// <param name="Id">Represents a standard ID that uniquely identifies the volunteer</param>
        /// <param name="FullName">Full name of the volunteer</param>
        /// <param name="Phone">Stands for a standard cell phone. 10 digits only. Starts with the number 0</param>
        /// <param name="Email">Represents a valid e-mail address in terms of format</param>
        /// <param name="Password">The volunteer's password</param>
        /// <param name="Address">Full and real address in correct format, of the volunteer</param>
        /// <param name="Latitude">A number that indicates how far a point on Earth is south or north of the equator</param>
        /// <param name="Longitude">A number that indicates how far a point on Earth is east or west of the prime meridian</param>
        /// <param name="Role">"Manager" or "Volunteer"</param>
        /// <param name="IsActive">If the volunteer is active or inactive (retired from the organization)</param>
        /// <param name="MaxDistance">A volunteer will define through the display the maximum distance to receive a call</param>
        /// <param name="TypeDistance">Aerial distance, walking distance, driving distance</param>
        /// <param name="TotalCallsHandled">Total number of calls handled by the volunteer</param>
        /// <param name="TotalCallsCancelled">Total number of calls canceled by the volunteer</param>
        /// <param name="TotalCallsExpired">Total number of calls that the volunteer chose to handle but expired</param>
        /// <param name="CurrentCall">Represents the current call in progress by the volunteer</param>
    public class Volunteer
    {
        public int Id { get; init; } 
        public string FullName { get; set; } 
        public string Phone { get; set; } 
        public string Email { get; set; } 
        public string? Password { get; set; } 
        public string? Address { get; set; } 
        public double? Latitude { get; set; } 
        public double? Longitude { get; set; }
        public MyRole Role { get; set; } 
        public bool IsActive { get; set; } 
        public double? MaxDistance { get; set; }
        public MyTypeDistance TypeDistance { get; set; } 
        public int TotalCallsHandled { get; init; } 
        public int TotalCallsCancelled { get; init; } 
        public int TotalCallsExpired { get; init; } 
        public BO.CallInProgress? CurrentCall { get; set; }

        internal static object FirstOrDefault(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}

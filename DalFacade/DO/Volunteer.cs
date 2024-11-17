namespace DO;
/// <summary>
/// Details about the volunteer and his attributes
/// </summary>
/// <param name="Id">represents a standard ID that uniquely identifies the volunteer</param>
/// <param name="FullName">Full name of the volunteer</param>
/// <param name="Phone">Stands for a standard cell phone. 10 digits only. Starts with the number 0</param>
/// <param name="Email">Represents a valid e-mail address in terms of format</param>
/// <param name="Role">"Manager" or "Volunteer"</param>
/// <param name="TypeDistance">Aerial distance, walking distance, driving distance</param>
/// <param name="Password">The volunteer's password</param>
/// <param name="Address">Full and real address in correct format, of the volunteer</param>
/// <param name="Latitude">a number that indicates how far a point on Earth is south or north of the equator</param>
/// <param name="Longitude">a number that indicates how far a point on Earth is south or north of the equator</param>
/// <param name="MaxDistance">A volunteer will define through the display the maximum distance to receive a call</param>
/// <param name="IsActive">If the volunteer is active or inactive (retired from the organization)</param>
public record Volunteer
(
 int Id,
string FullName,
string Phone,
string Email,
MyRole Role,
MyTypeDistance TypeDistance,
string? Password=null,
string? Address= null,
double? Latitude=null,
double? Longitude = null,
double? MaxDistance = null,
bool IsActive = false
)
{
   /// Default constructor for stage 3
public Volunteer():this(0,"","","",0,0) { }

    public override string ToString() { return $"ID: {Id}, Name: {FullName}, Phone: {Phone}, Email: {Email}, Role: {Role}, " + $"Type Distance: {TypeDistance}, Address: {Address}, Latitude: {Latitude}, " + $"Longitude: {Longitude}, Max Distance: {MaxDistance}, Active: {IsActive}"; }
}


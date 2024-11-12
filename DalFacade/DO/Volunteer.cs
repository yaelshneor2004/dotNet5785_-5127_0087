namespace DO;

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
}

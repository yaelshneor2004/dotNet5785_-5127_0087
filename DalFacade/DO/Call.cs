
using Microsoft.VisualBasic;

namespace DO;
/// <summary>
/// Details and features that each call has
/// </summary>
/// <param name="Id">represents a standard ID that uniquely identifies the volunteer</param>
/// <param name="CallType">according to the specific system type</param>
/// <param name="Address">Full and real address in correct format, of the reading location</param>
/// <param name="Latitude">a number that indicates how far a point on Earth is south or north of the equator</param>
/// <param name="Longitude">a number that indicates how far a point on Earth is south or north of the equator</param>
/// <param name="OpenTime">Time (date and time) when the call was opened by the manager</param>
/// <param name="Description">Description of the reading. Detailed details about the call</param>
/// <param name="MaxFinishCall">Time (date and time) by which the reading should be closed</param>
public record Call
 (
 int Id,
 MyCallType CallType,
 string Address,
 double Latitude,
double Longitude ,
DateTime OpenTime,
string? Description= null,
DateTime? MaxFinishCall= null
)
{
    /// Default constructor for stage 3
    public Call() : this(0,0, "", 0, 0,DateTime.Now) { }
}

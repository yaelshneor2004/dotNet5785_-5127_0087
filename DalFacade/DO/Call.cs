
using Microsoft.VisualBasic;

namespace DO;

public record Call
 (
 int Id,
 MyCallType CallType,
 string Address,
 double Latitude,
double Longitude ,
DateTime OpenTime,
string? Description=null,
DateTime? MaxFinishCall=null
)
{
    /// Default constructor for stage 3
    public Call() : this(0,0, "", 0, 0,DateTime.Now) { }
}

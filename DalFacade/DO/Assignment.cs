

namespace DO;

public record Assignment
    (
     int Id,
     int CallId,
     int VolunteerId,
     DateTime StartCall,
     Enum? FinishType=null,
     DateTime? FinishCall = null
  )
{
    /// Default constructor for stage 3
    public Assignment() : this(0, 0, 0, DateTime.Now) { }
}

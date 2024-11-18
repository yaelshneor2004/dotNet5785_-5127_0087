using System;
using System.Collections.Generic;

using DalApi;
using DO;

namespace DO;
/// <summary>
/// An entity that links "reading" to a "volunteer" who chose to take care of it
/// </summary>
/// <param name="Id"></param>
/// <param name="CallId">Represents a number that identifies the call that the volunteer chose to handle</param>
/// <param name="VolunteerId">represents the ID of the volunteer who chose to take care of the reading</param>
/// <param name="StartCall">Time (date and time) when the current call was processed</param>
/// <param name="FinishType">The manner in which the treatment of the current reading was completed by the current volunteer</param>
/// <param name="FinishCall">Time (date and time) when the current volunteer finished handling the current call</param>
public record Assignment
    (
     int Id,
     int CallId,
     int VolunteerId,
     DateTime StartCall,
     Enum? FinishType = null,
     DateTime? FinishCall = null
  )
{
    /// Default constructor for stage 3
    public Assignment() : this(0, 0, 0, DateTime.Now) { }

}


namespace BO;
/// <summary>
/// Enum representing roles within the organization.
/// Possible values are 'Volunteer' and 'Manager'
/// </summary>
public enum MyRole
{
    Volunteer,
    Manager
}

/// <summary>
/// Enum specifying types of distance calculations or travel modes.
/// Values include 'Aerial' for air distance, 'Walking', and 'Traveling' for vehicle-based movement
/// </summary>
public enum MyTypeDistance
{
    Aerial,
    Walking,
    Traveling
}

/// <summary>
/// Enum for categorizing call types in the system. 
/// </summary>
public enum MyCallType
{
    English,
    Math,
    ComputerScience,
    Accounting
}

/// <summary>
///  Enum representing completion or cancellation statuses for calls.
/// Options include 'Treated' for completed cases, 'SelfCancel' for user-initiated cancellations, 
/// 'ManagerCancel' for administrative cancellations, and 'ExpiredCancel' for time-out cases
/// </summary>
public enum MyCallStatusByVolunteer
{ 
    InProgress,
    AtRisk
}
public enum MyCurrentCallType
{
    Emergency,
    Regular,
    None // Added to handle cases where no call is in progress
}
public enum MyCallStatus
{
    Open,
    InProgress,
    Closed,
    Expired,
    OpenAtRisk,
    InProgressAtRisk
}
public enum MyFinishType
{
    Treated,
    SelfCancel,
    ManagerCancel,
    ExpiredCancel
}
public enum MySortInVolunteerInList
{
FullName,
 TotalCallsHandled ,
 TotalCallsCancelled ,
 CurrentCallType 
}
public enum MySortInCallInList
{
 Type ,
StartTime ,
 TimeRemaining ,
 CompletionTime ,
Status ,
 TotalAssignments 
}

public enum CloseCall
{
 
Address,
StartTime,
StartTreatmentTime,
EndTime,
 EndType
}

public enum OpenedCall
{
    Type ,
     Address ,
    StartTime, 
    MaxEndTime ,
   DistanceFromVolunteer 
    }
public enum Clock
{
    Minute,
    Hour,
    Day,
    Month,
    Year,
}
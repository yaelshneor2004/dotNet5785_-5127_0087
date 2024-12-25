namespace BO;

/// <summary>
/// Enum representing roles within the organization.
/// Possible values are 'Volunteer' and 'Manager'
/// </summary>
public enum MyRole
{
    Volunteer,
    Manager,
        None
}

/// <summary>
/// Enum specifying types of distance calculations or travel modes.
/// Values include 'Aerial' for air distance, 'Walking', and 'Traveling' for vehicle-based movement
/// </summary>
public enum MyTypeDistance
{
    Aerial,
    Walking,
    Traveling,
    None
}

/// <summary>
/// Enum for categorizing call types in the system. 
/// Values include 'English', 'Math', 'ComputerScience', and 'Accounting'
/// </summary>
public enum MyCallType
{
    English,
    Math,
    ComputerScience,
    Accounting
    ,None
}
/// <summary>
/// Enum representing completion or cancellation statuses for calls.
/// Options include 'InProgress' for ongoing cases, 'AtRisk' for at-risk cases
/// </summary>
public enum MyCallStatusByVolunteer
{
    InProgress,
    AtRisk,
    None
}

/// <summary>
/// Enum representing the status of calls.
/// Possible values are 'Open', 'InProgress', 'Closed', 'Expired', 'OpenAtRisk', and 'InProgressAtRisk'
/// </summary>
public enum MyCallStatus
{
    Open,
    InProgress,
    Closed,
    Expired,
    OpenAtRisk,
    InProgressAtRisk
}

/// <summary>
/// Enum representing the types of call completion or cancellation.
/// Values include 'Treated', 'SelfCancel', 'ManagerCancel', and 'ExpiredCancel'
/// </summary>
public enum MyFinishType
{
    Treated,
    SelfCancel,
    ManagerCancel,
    ExpiredCancel
}

/// <summary>
/// Enum for sorting volunteers in a list.
/// Options include 'FullName', 'TotalCallsHandled', 'TotalCallsCancelled', and 'CurrentCallType'
/// </summary>
public enum MySortInVolunteerInList
{    All,
    FullName,
    TotalCallsHandled,
    TotalCallsCancelled,
    CurrentCallType
}

/// <summary>
/// Enum for sorting calls in a list.
/// Values include 'Type', 'StartTime', 'TimeRemaining', 'CompletionTime', 'Status', and 'TotalAssignments'
/// </summary>
public enum MySortInCallInList
{
    All,
    Type,
    StartTime,
    TimeRemaining,
    CompletionTime,
    Status,
    TotalAssignments
}

/// <summary>
/// Enum for details of closed calls.
/// Includes 'Address', 'StartTime', 'StartTreatmentTime', 'EndTime', and 'EndType'
/// </summary>
public enum CloseCall
{
    Address,
    StartTime,
    StartTreatmentTime,
    EndTime,
    EndType
}

/// <summary>
/// Enum for details of opened calls.
/// Includes 'Type', 'Address', 'StartTime', 'MaxEndTime', and 'DistanceFromVolunteer'
/// </summary>
public enum OpenedCall
{
    Type,
    Address,
    StartTime,
    MaxEndTime,
    DistanceFromVolunteer
}

/// <summary>
/// Enum representing different units of time.
/// Options include 'Minute', 'Hour', 'Day', 'Month', and 'Year'
/// </summary>
public enum Clock
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}

/// <summary>
/// Enum for the first menu in the system.
/// Options include 'Exit', 'Volunteer', 'Call', 'Admin', 'InitializeData', 'ShowAllData', and 'ResetDatabaseAndConfig'
/// </summary>
public enum BoFirstMenu
{
    Exit,
    Volunteer,
    Call,
    Admin,
    InitializeData,
    ShowAllData,
    ResetDatabaseAndConfig
}

/// <summary>
/// Enum for showing volunteer menu options.
/// Values include 'Exit', 'Enter', 'AddVolunteer', 'DeleteVolunteer', 'GetVolunteerInLists', 'GetVolunteerDetails', and 'UpdateVolunteer'
/// </summary>
public enum VolunteerMenuShow
{
    Exit,
    Enter,
    AddVolunteer,
    DeleteVolunteer,
    GetVolunteerInLists,
    GetVolunteerDetails,
    UpdateVolunteer
}

/// <summary>
/// Enum for showing call menu options.
/// Options include 'Exit', 'GetCallCounts', 'ListOfCalls', 'GetCallDetails', 'UpdateCall', 'DeleteCall', 'AddCall', 'FilterClosedCallInLists', 'OpenCallInLists', 'EndTreatment', 'CancelTreatment', and 'ChooseCall'
/// </summary>
public enum CallMenuShow
{
    Exit,
    GetCallCounts,
    ListOfCalls,
    GetCallDetails,
    UpdateCall,
    DeleteCall,
    AddCall,
    FilterClosedCallInLists,
    OpenCallInLists,
    EndTreatment,
    CancelTreatment,
    ChooseCall
}

/// <summary>
/// Enum for showing admin menu options.
/// Values include 'Exit', 'SetRiskRange', 'GetClock', 'GetRiskRange', 'AdvanceClock', 'Initialization', and 'Reset'
/// </summary>
public enum AdminMenuShow
{
    Exit,
    SetRiskRange,
    GetClock,
    GetRiskRange,
    AdvanceClock,
    Initialization,
    Reset
}
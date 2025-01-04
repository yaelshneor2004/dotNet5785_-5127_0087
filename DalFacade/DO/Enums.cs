namespace DO;
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
    Accounting,
    None

}

/// <summary>
///  Enum representing completion or cancellation statuses for calls.
/// Options include 'Treated' for completed cases, 'SelfCancel' for user-initiated cancellations, 
/// 'ManagerCancel' for administrative cancellations, and 'ExpiredCancel' for time-out cases
/// </summary>
public enum MyFinishType
{
    Treated,
    SelfCancel,
    ManagerCancel,
    ExpiredCancel
}
/// <summary>
/// Enum containing options for the system configuration menu.
/// </summary>

public enum ConfigOptions
{
    Exit,
    AdvanceClockByMinute ,
    AdvanceClockByHour,
    ShowCurrentClock,
    SetRiskRange,
    ShowConfigValues,
    ResetConfig,
   
}
/// <summary>
/// Enum containing options for the main menu
/// </summary>
public enum MainMenuOptions
{
    Exit, ShowVolunteer, ShowCall, ShowAssignment, InitializeData, ShowAllData, ShowConfigSubMenu, ResetDatabaseAndConfig
}

/// <summary>
/// Enum representing CRUD (Create, Read, Update, Delete) operations and options
/// </summary>
public enum EntityMenuOption
{
    Exit, Create, Read, ReadAll, Update, Delete, DeleteAll
}

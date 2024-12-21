using BO;
using DalApi;
using DalTest;
using BlTest;
using DO;
using Microsoft.VisualBasic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Runtime.CompilerServices;
using BlApi;
using System.Security.Cryptography;
using System.Text;
using System;

namespace BlTest;
//yael shneor
internal class Program
{
    private static readonly Random s_rand = new(); // Random number generator

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private static IDal _dal = DalApi.Factory.Get; //stage 4
    private const string ApiKey = "6755cb286fc03337717882gdo28c286";
  static void Main(string[] args)
    {
        BoFirstMenu choice = BO.BoFirstMenu.Call;

        do
        {
            try
            {
                ShowFirstMenu();
                Console.WriteLine("enter your choice");
                string st = Console.ReadLine() ?? throw new DalInvalidOperationException("Invalid input. Please select a valid option.");
                if (!int.TryParse(st, out int parsedChoice)) throw new DalInvalidOperationException("Invalid input. Please enter a valid number.");
                choice = (BoFirstMenu)parsedChoice;

                // Processing the user's menu selection
                switch (choice)
                {
                    case (BoFirstMenu.Exit):
                        break;
                    case (BoFirstMenu.Volunteer):
                        VolunteerdMenu();
                        break;
                    case (BoFirstMenu.Call):
                        CallMenu();
                        break;
                    case (BoFirstMenu.Admin):
                        AdminMenu();
                        break;
                    case (BoFirstMenu.InitializeData):
                        Initialization.Do();
                        break;
                    case (BoFirstMenu.ShowAllData):
                        ReadAll("Volunteer");
                        ReadAll("Call");
                        ReadAll("Assignment");
                        break;
                    case (BoFirstMenu.ResetDatabaseAndConfig):
                        s_bl.Admin.Reset();
                        _dal.Assignment.DeleteAll();
                        _dal.Call.DeleteAll();
                        _dal.Volunteer.DeleteAll();
                        Console.WriteLine("The data has been reset");
                        break;
                    default:
                        Console.WriteLine("invalid option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // Handling exceptions
            }
        }
        while (choice != BoFirstMenu.Exit); // L
    }
    private static void ShowFirstMenu()
    {
        // Displaying main menu options
        Console.WriteLine("Please select an option:");
        Console.WriteLine("0 - Exit");
        Console.WriteLine("1 - Show volunteer menu");
        Console.WriteLine("2 - Show call menu");
        Console.WriteLine("3 - Show admin menu");
        Console.WriteLine("4 - initialize data");
        Console.WriteLine("5 - Show all data");
        Console.WriteLine("6 - reset database and config");
    }
    public static void ReadAll(string Type)
    {
        IEnumerable<object> entities = new List<object>();
        switch (Type)
        {
            case ("Volunteer"):
                entities = _dal.Volunteer.ReadAll().ToList();
                break;
            case ("Call"):
                entities = _dal.Call.ReadAll().ToList();
                break;
            case ("Assignment"):
                entities = _dal.Assignment.ReadAll().ToList();
                break;
        }
        Console.WriteLine($"{Type} List:");
        if (!entities.Any())
        {
            Console.WriteLine($"There is no entitis in {Type} list.");
        }
        foreach (var entity in entities)
        {
            Console.WriteLine(entity); // Displaying each entity in the list
        }
    }

    private static void VolunteerSubMenu()
    {
        // Displaying main menu options
        Console.WriteLine("Please select an option:");
        Console.WriteLine("0 - Exit");
        Console.WriteLine("1 - To enter");
        Console.WriteLine("2 - Add volunteer");
        Console.WriteLine("3 - Delete volunteer");
        Console.WriteLine("4 - Get list of volunteers");
        Console.WriteLine("5 - Get the details of volunteer");
        Console.WriteLine("6 - Update volunteer");
    }
    // Method to get filter field from the user
    private static BO.MySortInCallInList? GetFilterField()
    {
        Console.WriteLine("Enter the field to filter by:");
        Console.WriteLine("0 - Type");
        Console.WriteLine("1 - StartTime");
        Console.WriteLine("2 - TimeRemaining");
        Console.WriteLine("3 - CompletionTime");
        Console.WriteLine("4 - Status");
        Console.WriteLine("5 - TotalAssignments");
        string input = Console.ReadLine() ?? " ";
        if (!int.TryParse(input, out int option) || !Enum.IsDefined(typeof(MySortInCallInList), option))
        {
            return null;
        }
        return (BO.MySortInCallInList)option;
    }

    // Method to get sort field from the user
    private static BO.MySortInCallInList? GetSortField()
    {
        Console.WriteLine("Enter the field to sort by:");
        Console.WriteLine("0 - Type");
        Console.WriteLine("1 - StartTime");
        Console.WriteLine("2 - TimeRemaining");
        Console.WriteLine("3 - CompletionTime");
        Console.WriteLine("4 - Status");
        Console.WriteLine("5 - TotalAssignments");
        string input = Console.ReadLine() ?? " ";
        if (!int.TryParse(input, out int option) || !Enum.IsDefined(typeof(MySortInCallInList), option))
        {
            return null;
        }
        return (BO.MySortInCallInList)option;
    }

    // Method to get filter value from the user
    private static object? GetFilterValue()
    {
        Console.WriteLine("Enter the filter value (or press Enter to skip):");
        string input = Console.ReadLine();
        return string.IsNullOrEmpty(input) ? null : input;
    }

    public static void VolunteerdMenu()
    {
        VolunteerMenuShow choice;
        do
        {
            VolunteerSubMenu();
            string st = Console.ReadLine() ?? throw new DalInvalidOperationException("Invalid input. Please select a valid option.");
            if (!int.TryParse(st, out int parsedChoice)) throw new DalInvalidOperationException("Invalid input. Please enter a valid number.");
            choice = (VolunteerMenuShow)parsedChoice;

            // Processing the user's menu selection
            switch (choice)
            {
                case (VolunteerMenuShow.Exit):
                    break;
                case (VolunteerMenuShow.Enter):
                    Console.WriteLine("Enter your name:");
                    string userName = Console.ReadLine() ?? " ";
                    Console.WriteLine(" Enter your password");
                    string passward = Console.ReadLine() ?? " ";
                    Console.WriteLine(s_bl.Volunteer.Login(userName, passward));
                    break;
                case (VolunteerMenuShow.AddVolunteer):
                    BO.Volunteer volunteer = GetBOVolunteer();
                    s_bl.Volunteer.AddVolunteer(volunteer);
                    break;
                case (VolunteerMenuShow.DeleteVolunteer):
                    Console.WriteLine("Enter your id:");
                    string id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out int userId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    s_bl.Volunteer.DeleteVolunteer(userId);
                    break;
                case (VolunteerMenuShow.GetVolunteerInLists):
                    bool filter;
                    Console.WriteLine("Filter the volunteers according to active or inactive? ");
                    Console.WriteLine("0 - Active");
                    Console.WriteLine("1 - Inactive");
                    string input = Console.ReadLine() ?? " ";
                    if (!int.TryParse(input, out int filterOption)) Console.WriteLine("Invalid input. Please enter 0 for Active or 1 for Inactive.");
                    if (filterOption == 0)
                        filter = true;
                    else filter = false;
                    Console.WriteLine("How to sort the volunteers?");
                    Console.WriteLine("0 - FullName");
                    Console.WriteLine("1 - TotalCallsHandled");
                    Console.WriteLine("2 - TotalCallsCancelled");
                    Console.WriteLine("3 - CurrentCallType");
                    MySortInVolunteerInList sort;
                    st = Console.ReadLine() ?? throw new DalInvalidOperationException("Invalid input. Please select a valid option.");
                    if (!int.TryParse(st, out parsedChoice)) throw new DalInvalidOperationException("Invalid input. Please enter a valid number.");
                    sort = (MySortInVolunteerInList)parsedChoice;

                    switch (sort)
                    {
                        case MySortInVolunteerInList.FullName:
                            printVolunteers(s_bl.Volunteer.GetVolunteerList(filter, MySortInVolunteerInList.FullName));

                            break;

                        case MySortInVolunteerInList.TotalCallsHandled:
                            printVolunteers(s_bl.Volunteer.GetVolunteerList(filter, MySortInVolunteerInList.TotalCallsHandled));
                            break;

                        case MySortInVolunteerInList.TotalCallsCancelled:
                            printVolunteers(s_bl.Volunteer.GetVolunteerList(filter, MySortInVolunteerInList.TotalCallsCancelled));
                            break;
                        case MySortInVolunteerInList.CurrentCallType:
                            printVolunteers(s_bl.Volunteer.GetVolunteerList(filter, MySortInVolunteerInList.CurrentCallType));
                            break;

                        default:
                            throw new DalInvalidOperationException("Invalid input. Please select a valid option.");
                    }
                    break;
                case (VolunteerMenuShow.GetVolunteerDetails):
                    Console.WriteLine("Enter your id:");
                    id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out userId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    Console.WriteLine(s_bl.Volunteer.GetVolunteerDetails(userId));
                    break;
                case (VolunteerMenuShow.UpdateVolunteer):
                    Console.WriteLine("Enter your id:");
                    id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out userId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    volunteer = GetBOVolunteer();
                    s_bl.Volunteer.UpdateVolunteer(userId, volunteer);
                    Console.WriteLine("update succesful");
                    break;
                default:
                    Console.WriteLine("invalid option.");
                    break;
            }

        } while (choice != VolunteerMenuShow.Exit); // Looping until the user chooses to exit
    }
    private static void CallSubMenu()
    {
        // Displaying main menu options
        Console.WriteLine("Please select an option:");
        Console.WriteLine("0 - Exit");
        Console.WriteLine("1 - Request call quantities");
        Console.WriteLine("2 - Request list of calls");
        Console.WriteLine("3 - Request call details");
        Console.WriteLine("4 - Update call details");
        Console.WriteLine("5 - Delete call");
        Console.WriteLine("6 - Add call");
        Console.WriteLine("7 - Request list of closed calls handled by volunteer");
        Console.WriteLine("8 - Request list of open calls for volunteer to choose from");
        Console.WriteLine("9 - Update call as handled");
        Console.WriteLine("10 - Update call as canceled");
        Console.WriteLine("11 - Choose call for handling");

    }
    // Method to retrieve volunteer ID from the user
    private static int GetVolunteerID()
    {
        Console.WriteLine("Enter volunteer ID:");
        string id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
        if (!int.TryParse(id, out int volunteerId))
        {
            throw new FormatException("Invalid input. The ID must be a valid number.");
        }
        return volunteerId;
    }

    // Method to get call type from the user
    private static BO.MyCallType? GetCallTypeFilter()
    {
        Console.WriteLine("Enter the call type to filter by:");
        Console.WriteLine("0 - English");
        Console.WriteLine("1 - Math");
        Console.WriteLine("2 - ComputerScience");
        Console.WriteLine("3 - Accounting");
        string input = Console.ReadLine() ?? " ";
        if (!int.TryParse(input, out int option) || !Enum.IsDefined(typeof(BO.MyCallType), option))
        {
            return null;
        }
        return (BO.MyCallType)option;
    }

    // Method to get sort field from the user
    private static CloseCall? GetSortTypeFilter()
    {
        Console.WriteLine("Enter the field to sort by:");
        Console.WriteLine("0 - Address");
        Console.WriteLine("1 - StartTime");
        Console.WriteLine("2 - StartTreatmentTime");
        Console.WriteLine("3 - EndTime");
        Console.WriteLine("4 - EndType");
        string input = Console.ReadLine() ?? " ";
        if (!int.TryParse(input, out int option) || !Enum.IsDefined(typeof(CloseCall), option))
        {
            return null;
        }
        return (CloseCall)option;
    }
    // Method to get volunteer ID from the user
    private static int RetrieveVolunteerID()
    {
        Console.WriteLine("Enter volunteer ID:");
        string id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
        if (!int.TryParse(id, out int volunteerId))
        {
            throw new FormatException("Invalid input. The ID must be a valid number.");
        }
        return volunteerId;
    }

    // Method to get call type from the user
    private static BO.MyCallType? RetrieveCallTypeFilter()
    {
        Console.WriteLine("Enter the call type to filter by:");
        Console.WriteLine("0 - English");
        Console.WriteLine("1 - Math");
        Console.WriteLine("2 - ComputerScience");
        Console.WriteLine("3 - Accounting");
        string input = Console.ReadLine() ?? " ";
        if (!int.TryParse(input, out int option) || !Enum.IsDefined(typeof(BO.MyCallType), option))
        {
            return null;
        }
        return (BO.MyCallType)option;
    }

    // Method to get sort field from the user
    private static OpenedCall? RetrieveSortField()
    {
        Console.WriteLine("Enter the field to sort by:");
        Console.WriteLine("0 - Type");
        Console.WriteLine("1 - Address");
        Console.WriteLine("2 - StartTime");
        Console.WriteLine("3 - MaxEndTime");
        Console.WriteLine("4 - DistanceFromVolunteer");
        string input = Console.ReadLine() ?? " ";
        if (!int.TryParse(input, out int option) || !Enum.IsDefined(typeof(OpenedCall), option))
        {
            return null;
        }
        return (OpenedCall)option;
    }
    // Method to get volunteer ID from the user
    private static int GetVolunteerIdentifier()
    {
        Console.WriteLine("Enter volunteer ID:");
        string id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
        if (!int.TryParse(id, out int volunteerId))
        {
            throw new FormatException("Invalid input. The ID must be a valid number.");
        }
        return volunteerId;
    }

    // Method to get assignment ID from the user
    private static int GetAssignmentIdentifier()
    {
        Console.WriteLine("Enter assignment ID:");
        string id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
        if (!int.TryParse(id, out int assignmentId))
        {
            throw new FormatException("Invalid input. The ID must be a valid number.");
        }
        return assignmentId;
    }


    public static void CallMenu()
    {
        CallMenuShow choice;
        do
        {
            CallSubMenu();
            string st = Console.ReadLine() ?? throw new DalInvalidOperationException("Invalid input. Please select a valid option.");
            if (!int.TryParse(st, out int parsedChoice)) throw new DalInvalidOperationException("Invalid input. Please enter a valid number.");
            choice = (CallMenuShow)parsedChoice;
            // Processing the user's menu selection
            switch (choice)
            {
                case CallMenuShow.Exit:
                    break;
                case CallMenuShow.GetCallCounts:
                    int[] count = s_bl.Call.CallAmount();
                    foreach (MyCallStatus status in Enum.GetValues(typeof(MyCallStatus)))
                    {
                        int count1 = count[(int)status];
                        Console.WriteLine($"Status: {status}, Count: {count1}");
                    }
                    break;
                case CallMenuShow.ListOfCalls:
                    BO.MySortInCallInList? filterField = GetFilterField();
                    object? filterValue = GetFilterValue();
                    BO.MySortInCallInList? sortField = GetSortField();
                    var callList = s_bl.Call.GetCallList(filterField, filterValue, sortField);
                    foreach (var c in callList)
                    {
                        Console.WriteLine(c);
                    }
                    break;
                case CallMenuShow.GetCallDetails:
                    Console.WriteLine("Enter your id:");
                    string id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out int userVId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    Console.WriteLine(s_bl.Call.GetCallDetails(userVId));
                    break;
                case CallMenuShow.UpdateCall:
                    Console.WriteLine("Enter the number of the call you want to update:");
                    int idCall = int.Parse(Console.ReadLine());
                    BO.Call call = GetBOCall(idCall);
                    s_bl.Call.UpdateCall(call);

                    Console.WriteLine("Update successfully");
                    break;

                case CallMenuShow.DeleteCall:
                    Console.WriteLine("Enter your id:");
                    id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out int userId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    s_bl.Call.DeleteCall(userId);
                    Console.WriteLine("deleted successfully");
                    break;
                case CallMenuShow.AddCall:
                    call = GetBOCall();
                    s_bl.Call.AddCall(call);
                    Console.WriteLine(" successfully added");
                    break;
                case CallMenuShow.FilterClosedCallInLists:
                    int volunteerId = GetVolunteerID();
                    BO.MyCallType? callType = GetCallTypeFilter();
                    CloseCall? sortType = GetSortTypeFilter();

                    // Retrieve the filtered and sorted list
                    var closedCallList = s_bl.Call.SortClosedCalls(volunteerId, callType, sortType);

                    // Print the results
                    foreach (var c in closedCallList)
                    {
                        Console.WriteLine(c);
                    }
                    break;
                case CallMenuShow.OpenCallInLists:
                    int volId = RetrieveVolunteerID();
                    BO.MyCallType? selectedCallType = RetrieveCallTypeFilter();
                    OpenedCall? orderField = RetrieveSortField();

                    // Retrieve the filtered and sorted list of open calls
                    var openCalls = s_bl.Call.SortOpenedCalls(volId, selectedCallType, orderField);

                    // Print the results
                    foreach (var c in openCalls)
                    {
                        Console.WriteLine(c);
                    }
                    break;
                case CallMenuShow.EndTreatment:
                    int volunteerIdd = GetVolunteerIdentifier();
                    int assignmentId = GetAssignmentIdentifier();

                    // Update treatment end details
                    s_bl.Call.UpdateCompleteAssignment(volunteerIdd, assignmentId);
                    Console.WriteLine("The treatment has been successfully completed.");
                    break;

                case CallMenuShow.CancelTreatment:
                    Console.WriteLine("Enter your id:");
                    id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out userVId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    Console.WriteLine("Enter call id:");
                    string callid = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(callid, out int userCId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    s_bl.Call.UpdateCancelTreatment(userVId, userCId);
                    Console.WriteLine($"Cancellation of treatment {userCId} was successful.");
                    break;
                case CallMenuShow.ChooseCall:
                    Console.WriteLine("Enter your id:");
                    id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out userVId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    Console.WriteLine("Enter call id:");
                    callid = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(callid, out userCId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    s_bl.Call.SelectCallToTreat(userVId, userCId);
                    Console.WriteLine($"The addition of {userVId} was successful.");
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        } while (choice != CallMenuShow.Exit);
    }
    private static void AdminSubMenu()
    {
        // Displaying main menu options
        Console.WriteLine("Please select an option:");
        Console.WriteLine("0 - Exit");
        Console.WriteLine("1 - Set Risk Range");
        Console.WriteLine("2 - Get Clock");
        Console.WriteLine("3 - Get Risk Range");
        Console.WriteLine("4 - Advance Clock");
        Console.WriteLine("5 - initialization");
        Console.WriteLine("6 - Reset");

    }
    public static void AdminMenu()
    {
        AdminMenuShow choice;
        do
        {
            AdminSubMenu();
            string st = Console.ReadLine() ?? throw new DalInvalidOperationException("Invalid input. Please select a valid option.");
            if (!int.TryParse(st, out int parsedChoice)) throw new DalInvalidOperationException("Invalid input. Please enter a valid number.");
            choice = (AdminMenuShow)parsedChoice;
            // Processing the user's menu selection
            switch (choice)
            {
                case (AdminMenuShow.Exit):
                    break;
                case (AdminMenuShow.SetRiskRange):
                    Console.WriteLine("Enter new risk range (in minutes):");
                    string? riskRangeInput = Console.ReadLine();
                    if (int.TryParse(riskRangeInput, out int minutes))
                    {
                        s_bl.Admin.SetRiskRange(TimeSpan.FromMinutes(minutes));
                        Console.WriteLine($"Risk range: {minutes}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid number of minutes. Please enter a valid integer.");
                    }

                    break;
                case (AdminMenuShow.GetClock):
                    DateTime clock;
                    clock = s_bl.Admin.GetClock();
                    Console.WriteLine($"Clock: {clock}");
                    break;
                case (AdminMenuShow.GetRiskRange):
                    TimeSpan riskRange = s_bl.Admin.GetRiskRange();
                    Console.WriteLine($"Risk range: {riskRange}");
                    break;
                case (AdminMenuShow.AdvanceClock):
                    Clock advance;
                    Console.WriteLine("By how much time would you like to advance the clock?");
                    Console.WriteLine("0 - Minutes:");
                    Console.WriteLine("1 - Hour:");
                    Console.WriteLine("2 - Day:");
                    Console.WriteLine("3 - Month:");
                    Console.WriteLine("4 - Year:");
                    st = Console.ReadLine() ?? throw new DalInvalidOperationException("Invalid input. Please select a valid option.");
                    if (!int.TryParse(st, out parsedChoice)) throw new DalInvalidOperationException("Invalid input. Please enter a valid number.");
                    //Console.WriteLine("enter in how many time you want to advance");
                    //int num =int.Parse(Console.ReadLine());
                    advance = (Clock)parsedChoice;
                    switch (advance)
                    {
                        case Clock.Minute:
                            s_bl.Admin.AdvanceClock(Clock.Minute);
                            break;
                        case Clock.Hour:
                            s_bl.Admin.AdvanceClock(Clock.Hour);
                            break;
                        case Clock.Day:
                            s_bl.Admin.AdvanceClock(Clock.Day);
                            break;
                        case Clock.Month:
                            s_bl.Admin.AdvanceClock(Clock.Month);
                            break;
                        case Clock.Year:
                            s_bl.Admin.AdvanceClock(Clock.Year);
                            break;
                        default:
                            throw new DalInvalidOperationException("Invalid selection.");
                    }
                    break;
                case (AdminMenuShow.Initialization):
                    s_bl.Admin.initialization();
                    break;
                case (AdminMenuShow.Reset):
                    s_bl.Admin.Reset();
                    break;
                default:
                    Console.WriteLine("invalid option.");
                    break;
            }
        } while (choice != AdminMenuShow.Exit);

    }
    public static BO.Volunteer GetBOVolunteer()
    {

        Console.WriteLine("Enter your id:");
        if (!int.TryParse(Console.ReadLine(), out int id))
            throw new DalInvalidOperationException("Invalid ID! Please enter a valid integer.");

        Console.WriteLine("Enter full name:");
        string fullName = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter phone number:");
        string phoneNumber = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter email:");
        string email = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter your role:");
        Console.WriteLine("For Manager press 0:");
        Console.WriteLine("For Volunteer press 1:");
        if (!int.TryParse(Console.ReadLine(), out int roleInput) || !Enum.IsDefined(typeof(BO.MyRole), roleInput))
            throw new DalInvalidOperationException("Invalid role! Please select 0 for Manager or 1 for Volunteer.");
        BO.MyRole userRole = (BO.MyRole)roleInput;

        Console.WriteLine("Enter distance type:");
        Console.WriteLine("For Air distance press 0");
        Console.WriteLine("For Walk distance press 1");
        Console.WriteLine("For Drive distance press 2");
        if (!int.TryParse(Console.ReadLine(), out int distanceInput) || !Enum.IsDefined(typeof(BO.MyTypeDistance), distanceInput))
            throw new DalInvalidOperationException("Invalid distance type! Please select a valid option.");
        BO.MyTypeDistance selectedDistanceType = (BO.MyTypeDistance)distanceInput;
        string strongPassword = GenerateValidPassword();
        Console.WriteLine($"Generated Password: {strongPassword}");
        Console.Write("Press 0 to save the password or 1 to change the password: ");
        string userChoice = Console.ReadLine() ?? "";
        switch (userChoice)
        {
            case "0":
                Console.WriteLine("Password saved successfully.");
                break;
            case "1":
                Console.WriteLine("Please enter new Password");
                strongPassword = Console.ReadLine() ?? "";
                break;
            default:
                Console.WriteLine("Invalid choice. Password was not saved.");
                break;
        }
        Console.WriteLine("Enter address:");
        string address = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter MaxCallDistance:");
        if (!double.TryParse(Console.ReadLine(), out double maxCallDistance))
            throw new DalInvalidOperationException("Invalid MaxCallDistance! Please enter a valid number.");
        int TreatedCalls = GetTreatedCalls(id);
        int DeletedCalls = GetDeletedCalls(id);
        int ExpiredCalls = GetExpiredCalls(id);
        BO.Volunteer volunteer = new BO.Volunteer(id, fullName, phoneNumber, email, strongPassword, address, 0, 0, userRole, true, maxCallDistance, selectedDistanceType, TreatedCalls, DeletedCalls, ExpiredCalls, null);

        return volunteer; // Returns the newly created Volunteer object
    }
    public static BO.Call GetBOCall(int idC)
    {
        Console.WriteLine("Enter call type:");
        Console.WriteLine("0 - English");
        Console.WriteLine("1 - Math");
        Console.WriteLine("2 - ComputerScience");
        Console.WriteLine("3 - Accounting");

        if (!int.TryParse(Console.ReadLine(), out int callTypeInput) ||
            !Enum.IsDefined(typeof(BO.MyCallType), callTypeInput))
            throw new DalInvalidOperationException("Invalid call type! Please select a valid option.");

        BO.MyCallType boCallType = (BO.MyCallType)callTypeInput;
        DO.MyCallType doCallType = (DO.MyCallType)boCallType;

        Console.WriteLine("Enter call description:");
        string description = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter call address:");
        string address = Console.ReadLine() ?? string.Empty;
        DateTime callOpenTime = s_bl.Admin.GetClock();
        Console.WriteLine("enter in how many times do you wamt the call will close");
        DateTime? callMaxCloseTime = Convert.ToDateTime(Console.ReadLine());
        BO.MyCallStatus callStatus = BO.MyCallStatus.Open;
        BO.Call call = new BO.Call(idC, (BO.MyCallType)doCallType, address, 0, 0, callOpenTime, callMaxCloseTime, description, callStatus, null);
        return call; // Returns the newly created Call object
    }
    public static BO.Call GetBOCall()
    {
        Console.WriteLine("Enter call type:");
        Console.WriteLine("0 - English");
        Console.WriteLine("1 - Math");
        Console.WriteLine("2 - ComputerScience");
        Console.WriteLine("3 - Accounting");

        if (!int.TryParse(Console.ReadLine(), out int callTypeInput) ||
            !Enum.IsDefined(typeof(BO.MyCallType), callTypeInput))
            throw new DalInvalidOperationException("Invalid call type! Please select a valid option.");

        BO.MyCallType boCallType = (BO.MyCallType)callTypeInput;
        DO.MyCallType doCallType = (DO.MyCallType)boCallType;

        Console.WriteLine("Enter call description:");
        string description = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter call address:");
        string address = Console.ReadLine() ?? string.Empty;
        DateTime callOpenTime = s_bl.Admin.GetClock();
        Console.WriteLine("enter in how many times do you wamt the call will close");
        DateTime? callMaxCloseTime = Convert.ToDateTime(Console.ReadLine());
        BO.MyCallStatus callStatus = BO.MyCallStatus.Open;
        BO.Call call = new BO.Call(0, (BO.MyCallType)doCallType, address, 0, 0, callOpenTime, callMaxCloseTime, description, callStatus, null);
        return call; // Returns the newly created Call object
    }
    public static BO.MyCallStatus SetCallStatus(DO.Call doCall)
    {
        var assigment = _dal.Assignment.Read(doCall.Id);
        if (assigment == null)
        {
            if (doCall.MaxFinishCall.HasValue && (doCall.MaxFinishCall.Value - DateTime.Now) <= _dal.Config.RiskRange)
            {
                return BO.MyCallStatus.OpenAtRisk;
            }
            else if (doCall.MaxFinishCall.HasValue && DateTime.Now > doCall.MaxFinishCall.Value)
            {
                return BO.MyCallStatus.Expired;
            }
            else
            {
                return BO.MyCallStatus.Open;
            }
        }
        else
        {
            if (doCall.MaxFinishCall.HasValue && (doCall.MaxFinishCall.Value - DateTime.Now) <= _dal.Config.RiskRange)
            {
                return BO.MyCallStatus.InProgressAtRisk;
            }
            if (assigment.FinishType == DO.MyFinishType.ExpiredCancel)
            {
                return BO.MyCallStatus.Expired;
            }
            if (assigment.FinishType == DO.MyFinishType.Treated)
            {
                return BO.MyCallStatus.Closed;
            }
            if (assigment.FinishType == DO.MyFinishType.ManagerCancel || assigment.FinishType == DO.MyFinishType.SelfCancel)
            {
                return BO.MyCallStatus.Open;
            }
        }
        return BO.MyCallStatus.Expired;
    }
    public static int GetTreatedCalls(int id)
    {
        var assignmentList = _dal.Assignment.ReadAll();
        int count = assignmentList.Where(a => a.VolunteerId == id
        && a.FinishType == DO.MyFinishType.Treated).Count();
        return count;
    }
    public static int GetDeletedCalls(int id)
    {
        var assignmentList = _dal.Assignment.ReadAll();
        int count = assignmentList.Where(a => a.VolunteerId == id
        && a.FinishType == DO.MyFinishType.SelfCancel).Count();
        return count;
    }
    public static int GetExpiredCalls(int id)
    {
        var assignmentList = _dal.Assignment.ReadAll();
        int count = assignmentList.Where(a => a.VolunteerId == id
        && a.FinishType == DO.MyFinishType.ExpiredCancel).Count();
        return count;

    }

    public static BO.MyCallStatusByVolunteer DetermineCallStatus(int id)
    {
        DO.Call? call = _dal.Call.Read(id);
        if (call.MaxFinishCall - DateTime.Now <= _dal.Config.RiskRange)
        {
            return BO.MyCallStatusByVolunteer.AtRisk;
        }
        return BO.MyCallStatusByVolunteer.InProgress;
    }
    private static void printVolunteers(IEnumerable<VolunteerInList> listV)
    {
        foreach (var v in listV)
            Console.WriteLine(v + " ");
    }
    //Create a strong password
    private static string GenerateValidPassword()
    {

        string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string lowercase = "abcdefghijklmnopqrstuvwxyz";
        string digits = "0123456789";
        string specialChars = "@#$%^&*!";


        StringBuilder password = new StringBuilder();


        password.Append(uppercase[s_rand.Next(uppercase.Length)]);
        password.Append(lowercase[s_rand.Next(lowercase.Length)]);
        password.Append(digits[s_rand.Next(digits.Length)]);
        password.Append(specialChars[s_rand.Next(specialChars.Length)]);


        int passwordLength = s_rand.Next(8, 21);


        string allChars = uppercase + lowercase + digits + specialChars;
        for (int i = password.Length; i < passwordLength; i++)
        {
            password.Append(allChars[s_rand.Next(allChars.Length)]);
        }
        return new string(password.ToString().OrderBy(c => s_rand.Next()).ToArray());
    }
}


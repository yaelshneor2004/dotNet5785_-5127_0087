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

namespace BlTest;

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private static IDal _dal =DalApi.Factory.Get; //stage 4
    private const string ApiKey = "6755cb286fc03337717882gdo28c286";
    private const string GoogleMapsApiUrl = "https://maps.googleapis.com/maps/api/distancematrix/json";
    private const string GoogleGeocodingApiUrl = "https://maps.googleapis.com/maps/api/geocode/json";
    static void Main(string[] args)
    {
        try
        {
            BoFirstMenu choice;
            do
            {
                ShowFirstMenu();
                Console.WriteLine("enter your chice");
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
            } while (choice != BoFirstMenu.Exit); // L

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex); // Handling exceptions
            ShowFirstMenu();
        }
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
        IEnumerable<object> entities = new List<object>(); ;
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
            Console.WriteLine(entity); // Displaying each entity in the list
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
        Console.WriteLine("4 - GEt list of volunteers");
        Console.WriteLine("5 - GEt the details of volunteer");
        Console.WriteLine("6 - Update volunteer");
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
                    string input = Console.ReadLine()??" ";
                    if (!int.TryParse(input, out int filterOption)) Console.WriteLine("Invalid input. Please enter 0 for Active or 1 for Inactive.");
                    if (filterOption == 0)
                        filter = true;
                    else filter = false;
                    Console.WriteLine("How to sort the volunteers?");
                    Console.WriteLine("0 - Id");
                    Console.WriteLine("1 - FullName");
                    Console.WriteLine("2 - CallsHandled");
                    Console.WriteLine("3 - CallsCancelled");
                    Console.WriteLine("4 - ExpiredCalls");
                    Console.WriteLine("5 - CallId");
                    Console.WriteLine("6 - CallType");
                    MySortInVolunteerInList sort;
                    st = Console.ReadLine() ?? throw new DalInvalidOperationException("Invalid input. Please select a valid option.");
                    if (!int.TryParse(st, out parsedChoice)) throw new DalInvalidOperationException("Invalid input. Please enter a valid number.");
                    sort = (MySortInVolunteerInList)parsedChoice;

                    switch (sort)
                    {
                        case MySortInVolunteerInList.FullName:
                            s_bl.Volunteer.GetVolunteerList(filter, MySortInVolunteerInList.FullName);
                            break;

                        case MySortInVolunteerInList.TotalCallsHandled:
                            s_bl.Volunteer.GetVolunteerList(filter, MySortInVolunteerInList.TotalCallsHandled);
                            break;

                        case MySortInVolunteerInList.TotalCallsCancelled:
                            s_bl.Volunteer.GetVolunteerList(filter, MySortInVolunteerInList.TotalCallsCancelled);
                            break;
                        case MySortInVolunteerInList.CurrentCallType:
                            s_bl.Volunteer.GetVolunteerList(filter, MySortInVolunteerInList.CurrentCallType);
                            break;

                        default:
                            throw new DalInvalidOperationException("Invalid input. Please select a valid option.");
                    }
                    break;
                case (VolunteerMenuShow.GetVolunteerDetails):
                    Console.WriteLine("Enter your id:");
                    id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out userId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    s_bl.Volunteer.GetVolunteerDetails(userId);
                    break;
                case (VolunteerMenuShow.UpdateVolunteer):
                    Console.WriteLine("Enter your id:");
                    id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out userId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    volunteer = GetBOVolunteer();
                    s_bl.Volunteer.UpdateVolunteer(userId, volunteer);
                    break;
                default:
                    Console.WriteLine("invalid option.");
                    break;
            }
        } while (choice != VolunteerMenuShow.Exit); // Looping until the user chooses to exit
    }
    public static void CallMenu()
    {
        CallMenuShow choice;
        do
        {
            // Displaying main menu options
            Console.WriteLine("Please select an option:");
            Console.WriteLine("0 - Exit");
            Console.WriteLine("1 - To enter");
            Console.WriteLine("2 - Add volunteer");
            Console.WriteLine("3 - Delete volunteer");
            Console.WriteLine("4 - GEt list of volunteers");
            Console.WriteLine("5 - GEt the details of volunteer");
            Console.WriteLine("6 - Update volunteer");

            string st = Console.ReadLine() ?? throw new DalInvalidOperationException("Invalid input. Please select a valid option.");
            if (!int.TryParse(st, out int parsedChoice)) throw new DalInvalidOperationException("Invalid input. Please enter a valid number.");
            choice = (CallMenuShow)parsedChoice;
            // Processing the user's menu selection
            switch (choice)
            {
                case (CallMenuShow.Exit):
                    break;
                case (CallMenuShow.AddCall):
                    BO.Call call = GetBOCall();
                    s_bl.Call.AddCall(call);
                    break;
                case (CallMenuShow.DeleteCall):
                    Console.WriteLine("Enter your id:");
                    string id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out int userId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    s_bl.Call.DeleteCall(userId);
                    break;
                case (CallMenuShow.CancelTreatment):
                    Console.WriteLine("Enter your id:");
                    id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out int userVId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    Console.WriteLine("Enter call ud:");
                    string callid = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(callid, out int userCId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    s_bl.Call.SelectCallToTreat(userVId, userCId);
                    break;
                case (CallMenuShow.FilterClosedCallInLists):

                    break;
                case (CallMenuShow.EndTreatment):

                    break;
                case (CallMenuShow.GetCallDetails):
                    Console.WriteLine("Enter your id:");
                    id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out userVId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    s_bl.Call.GetCallDetails(userVId);
                    break;
                case (CallMenuShow.GetCallCounts):
                    int[] count = s_bl.Call.CallAmount();
                    foreach (int c in count)
                    {
                        Console.WriteLine(c);
                    }
                    break;
                case (CallMenuShow.ListOfCalls):

                    break;
                case (CallMenuShow.OpenCallInLists):

                    break;
                case (CallMenuShow.ChooseCall):
                    Console.WriteLine("Enter your id:");
                    id = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(id, out userVId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    Console.WriteLine("Enter call ud:");
                    callid = Console.ReadLine() ?? throw new FormatException("Invalid input. The ID must be a valid number.");
                    if (!int.TryParse(callid, out userCId)) throw new FormatException("Invalid input. The ID must be a valid number.");
                    s_bl.Call.SelectCallToTreat(userVId, userCId);
                    break;
                case (CallMenuShow.UpdateCall):
                    call = GetBOCall();
                    s_bl.Call.UpdateCall(call);
                    break;
                default:
                    Console.WriteLine("invalid option.");
                    break;
            }



        } while (choice != CallMenuShow.Exit);
    }
    public static void AdminMenu()
    {
        AdminMenuShow choice;
        do
        {
            // Displaying main menu options
            Console.WriteLine("Please select an option:");
            Console.WriteLine("0 - Exit");
            Console.WriteLine("1 - Set Clock");
            Console.WriteLine("2 - Set Risk Range");
            Console.WriteLine("3 - Get Clock");
            Console.WriteLine("4 - Get Risk Range");
            Console.WriteLine("5 - Advance Clock");
            Console.WriteLine("6 - initialization");
            Console.WriteLine("7 - Reset");

            string st = Console.ReadLine() ?? throw new DalInvalidOperationException("Invalid input. Please select a valid option.");
            if (!int.TryParse(st, out int parsedChoice)) throw new DalInvalidOperationException("Invalid input. Please enter a valid number.");
            choice = (AdminMenuShow)parsedChoice;
            // Processing the user's menu selection
            switch (choice)
            {
                case (AdminMenuShow.Exit):
                    break;
                case (AdminMenuShow.SetClock):
                    Console.WriteLine("Enter new clock (HH:mm:ss):");
                    string clockInput = Console.ReadLine() ?? throw new DalInvalidOperationException("Invalid input. Enter new clock (HH:mm:ss):");
                    DateTime clock;
                    if (DateTime.TryParse(clockInput, out clock))
                    {
                        //s_bl.Admin.SetRiskRange(clock);
                        Console.WriteLine($"Clock: {clock}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid time format. Please enter a valid time.");
                    }
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

        Console.WriteLine("Enter password:");
        string password = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter address:");
        string address = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter MaxCallDistance:");
        if (!double.TryParse(Console.ReadLine(), out double maxCallDistance))
            throw new DalInvalidOperationException("Invalid MaxCallDistance! Please enter a valid number.");
        int TreatedCalls = GetTreatedCalls(id);
        int DeletedCalls = GetDeletedCalls(id);
        int ExpiredCalls = GetExpiredCalls(id);
        BO.CallInProgress? CallOnTreatment = GetCallOnTreatment(id);

        BO.Volunteer volunteer = new BO.Volunteer ( id,  fullName,phoneNumber, email, password, address, 0, 0, userRole, true,  maxCallDistance,  selectedDistanceType, TreatedCalls,   DeletedCalls,  ExpiredCalls );

        return volunteer; // Returns the newly created Volunteer object
    }
    public static BO.Call GetBOCall()
    {
        Console.WriteLine("Enter Call id:");
        if (!int.TryParse(Console.ReadLine(), out int Id))
            throw new DalDoesNotExistException($"Assignment with id={Id} does not exist");

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

        Console.WriteLine("Enter latitude:");
        if (!double.TryParse(Console.ReadLine(), out double latitude))
            throw new DalInvalidOperationException("Invalid latitude! Please enter a valid number.");

        Console.WriteLine("Enter longitude:");
        if (!double.TryParse(Console.ReadLine(), out double longitude))
            throw new DalInvalidOperationException("Invalid longitude! Please enter a valid number.");


        DateTime callOpenTime = s_bl.Admin.GetClock();
        DateTime? callMaxCloseTime = s_bl.Admin.GetClock();
        DO.Call tmpcall = new DO.Call()
        {
            CallType = doCallType,
            Id = Id,
           Address = address,
            Latitude = latitude,
            Longitude = longitude,
            OpenTime = callOpenTime,
            MaxFinishCall = callMaxCloseTime,
            Description = description
        };
        BO.MyCallStatus callStatus = SetCallStatus(tmpcall);
        List<BO.CallAssignInList>? callAssignments = CreateFilteredCallAssignList(Id);
        BO.Call call = new BO.Call(Id, (BO.MyCallType)doCallType, address, latitude, longitude, callOpenTime, callMaxCloseTime, description, callStatus, callAssignments);
        return call; // Returns the newly created Call object
    }
    public static List<BO.CallAssignInList>? CreateFilteredCallAssignList(int id)
    {
        var allAssignments = _dal.Assignment.ReadAll()
            .Where(assignment => assignment.CallId == id)
            .Select(assignment => MapAssignmentDOtoAssignmentBO(assignment))
            .ToList();

        return allAssignments;
    }
    public static BO.CallAssignInList MapAssignmentDOtoAssignmentBO(DO.Assignment doAssignment)
    {
        var valunterr = _dal.Volunteer.Read(doAssignment.VolunteerId);
        if (doAssignment == null)
        {
            return null;
        }
        return new BO.CallAssignInList
        {
            VolunteerId = doAssignment.Id,
           VolunteerName = valunterr.FullName,
            StartTreatmentTime = doAssignment.StartCall,
            EndTreatmentTime = doAssignment.FinishCall,
            EndType = (BO.MyFinishType?)(doAssignment.FinishType ?? default(DO.MyFinishType)),
        };
    }
    public static BO.MyCallStatus SetCallStatus(DO.Call doCall)
    {
        var assigment = _dal.Assignment.Read(doCall.Id);
        if (assigment == null)
        {
            if (doCall.MaxFinishCall.HasValue && (doCall.MaxFinishCall.Value -DateTime.Now) <= _dal.Config.RiskRange)
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
    public static BO.CallInProgress? GetCallOnTreatment(int id)
    {
        int count = GetTreatedCalls(id);
        if (count > 0)
        {
            DO.Assignment? assignment = _dal.Assignment.Read(id);
            DO.Volunteer? volunteer = _dal.Volunteer.Read(id);
            DO.Call? call = _dal.Call.Read(assignment.CallId);

            return new BO.CallInProgress
            {
                Id = assignment.Id,
                CallId = call.Id,
                CallType = (BO.MyCallType)call.CallType,
                Description = call.Description,
                Address = call.Address,
                StartTime = call.OpenTime,
                MaxEndTime = call.MaxFinishCall,
                StartTreatmentTime = assignment.StartCall,
                DistanceFromVolunteer = CalculateDistance(volunteer.TypeDistance, volunteer.Latitude ?? 0, volunteer.Longitude ?? 0, call.Latitude, call.Longitude),
                Status = DetermineCallStatus(id)

            };
        }
        return null;

    }
    public static double CalculateDistance(DO.MyTypeDistance type, double? volunteerLatiude, double? volunteerLongitude, double? callLatitude, double? callLongitude)
    {

        switch (type)
        {

            case DO.MyTypeDistance.Walking:
                if (!volunteerLatiude.HasValue || !volunteerLongitude.HasValue || !callLatitude.HasValue || !callLongitude.HasValue)
                {
                    throw new ArgumentException("All coordinates must have valid values.");
                }

                // בונים את ה-URL של הבקשה
                var url1 = $"{GoogleMapsApiUrl}?origins={volunteerLatiude.Value},{volunteerLongitude.Value}&destinations={callLatitude.Value},{callLongitude.Value}&mode=walking&key={ApiKey}";

                // יצירת HttpClient וביצוע הבקשה בצורה סינכרונית
                using (var client = new HttpClient())
                {
                    var response = client.GetStringAsync(url1).Result; // קריאה סינכרונית

                    // פירוש התשובה ב- JSON
                    var jsonResponse = JsonDocument.Parse(response);
                    var distance = jsonResponse.RootElement
                            .GetProperty("rows")[0]
                            .GetProperty("elements")[0]
                            .GetProperty("distance")
                            .GetProperty("value")
                            .GetInt32();
                    return distance / 1000.0;
                }

            case DO.MyTypeDistance.Traveling:
                if (!volunteerLatiude.HasValue || !volunteerLongitude.HasValue || !callLatitude.HasValue || !callLongitude.HasValue)
                {
                    throw new ArgumentException("All coordinates must have valid values.");
                }

                // בונים את ה-URL של הבקשה עם פרמטרים של הקואורדינטות וה-API Key
                var url2 = $"{GoogleMapsApiUrl}?origins={volunteerLatiude.Value},{volunteerLongitude.Value}&destinations={callLatitude.Value},{callLongitude.Value}&mode=driving&key={ApiKey}";

                // יצירת HttpClient וביצוע הבקשה בצורה סינכרונית
                using (var client = new HttpClient())
                {
                    var response = client.GetStringAsync(url2).Result; // קריאה סינכרונית (ללא async/await)

                    // פירוש התשובה ב- JSON בעזרת System.Text.Json
                    var jsonResponse = JsonDocument.Parse(response);  // ניתוח ה- JSON בעזרת JsonDocument
                    var distance = jsonResponse.RootElement
                                               .GetProperty("rows")[0]
                                               .GetProperty("elements")[0]
                                               .GetProperty("distance")
                                               .GetProperty("value")
                                               .GetInt32();

                    // המר mesק ממטרים לקילומטרים (המרה ממטרים לקילומטרים)
                    return distance / 1000.0; // המרחק בקילומטרים
                }


            case DO.MyTypeDistance.Aerial:
                const double R = 6371; // רדיוס כדור הארץ בקילומטרים

                // חישוב ההבדלים בין הקואורדינטות
                double latDistance = DegreesToRadians(callLatitude.Value - volunteerLatiude.Value);
                double lonDistance = DegreesToRadians(callLongitude.Value - volunteerLongitude.Value);

                // חישוב Haversine
                double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2) +
                           Math.Cos(DegreesToRadians(volunteerLatiude.Value)) * Math.Cos(DegreesToRadians(callLatitude.Value)) *
                           Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

                // חישוב המרחק בקילומטרים
                return R * c;

        }
        return 0;

    }
    public static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
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
}


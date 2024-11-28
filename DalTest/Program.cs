using Dal;
using DalApi;
using DO;

namespace DalTest;

internal class Program
{
    //static readonly IDal s_dal = new DalList();
    static readonly IDal s_dal = new Dal.DalXml();

    public static void Main(string[] args)
    {
        try
        {
            MainMenuOptions selectedOption;
            do
            {
                ShowMainMenu(); // Show the main menu
                if (!int.TryParse(Console.ReadLine(), out int selectedOptionInt)) throw new DalFormatException("Invalid input for menu option!");
                selectedOption = (MainMenuOptions)selectedOptionInt;
                switch (selectedOption)
                {
                    case MainMenuOptions.ShowVolunteer:
                        VolunteerEntityMenu();// Show volunteer submenu
                        break;
                    case MainMenuOptions.ShowCall:
                        CallEntityMenu(); // Show call submenu
                        break;
                    case MainMenuOptions.ShowAssignment:
                        AssignmentEntityMenu(); // Show assignment submenu
                        break;
                    case MainMenuOptions.InitializeData:
                        InitializeData(); // Initialize data
                        break;
                    case MainMenuOptions.ShowAllData:
                        ShowAllData(); // Show all data
                        break;
                    case MainMenuOptions.ShowConfigSubMenu:
                        ShowConfigSubMenu(); // Show config submenu
                        break;
                    case MainMenuOptions.ResetDatabaseAndConfig:
                        ResetDatabaseAndConfig(); // Reset database and config
                        break;
                    default:
                        throw new DalInvalidOperationException("Invalid option. Please try again.");

                }


            }
            while (selectedOption != 0);
        }
        catch (DalDoesNotExistException ex) { Console.WriteLine($"Data access error: {ex.Message}"); ShowMainMenu(); }
        catch (DalAlreadyExistsException ex) { Console.WriteLine($"Data access error: {ex.Message}"); ShowMainMenu(); }
        catch (DalDeletionImpossible ex) { Console.WriteLine($"Data access error: {ex.Message}"); ShowMainMenu(); }
        catch (DalFormatException ex) { Console.WriteLine($"Input error: {ex.Message}"); ShowMainMenu(); }
        catch (InvalidOperationException ex) { Console.WriteLine($"Operation error: {ex.Message}"); ShowMainMenu(); }
    }

    //The SubMenuConfig function displays a configuration submenu with options for advancing the system clock, showing or setting configuration values, and resetting configuration values
    private static void ShowMainMenu()
    {
        Console.WriteLine("Main Menu:");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Displaying a submenu for the volunteers");
        Console.WriteLine("2. Displaying a submenu for the Call");
        Console.WriteLine("3. Displaying a submenu for the Assignment");
        Console.WriteLine("4. Initialize Data");
        Console.WriteLine("5. Show All Data");
        Console.WriteLine("6. Show Config Sub-Menu");
        Console.WriteLine("7. Reset Database and Config");
        Console.Write("Select an option: ");
    }

    private static void ShowConfigSubMenu()
    {
        ConfigOptions selectedOption;
        do
        {
            SubMenuConfig(); // Show configuration submenu
            if (!int.TryParse(Console.ReadLine(), out int selectedOptionInt)) throw new DalFormatException("Invalid input for menu option!");
            selectedOption = (ConfigOptions)selectedOptionInt;
            switch (selectedOption)
            {
                case ConfigOptions.AdvanceClockByMinute:
                    // Advance System Clock by a Minute
                    var clock = s_dal?.Config.Clock ?? throw new ConfigNotFoundException("s_dal.Config is null");
                    s_dal.Config.Clock = clock.AddMinutes(1);
                    Console.WriteLine($"New System Clock: {s_dal.Config.Clock}");
                    break;
                case ConfigOptions.AdvanceClockByHour:
                    // Advance System Clock by an Hour
                    clock = s_dal?.Config.Clock ?? throw new ConfigNotFoundException("s_dal.Config is null");
                    s_dal.Config.Clock = clock.AddHours(1);
                    Console.WriteLine($"New System Clock: {s_dal.Config.Clock}");
                    break;
                case ConfigOptions.ShowCurrentClock:
                    // Show Current System Clock
                    clock = s_dal?.Config.Clock ?? throw new ConfigNotFoundException("s_dal.Config is null");
                    Console.WriteLine($"Current System Clock: {clock}");
                    break;
                case ConfigOptions.SetRiskRange:
                    SetRiskRange(); // Set the risk range
                    break;
                case ConfigOptions.ShowConfigValues:
                    ShowConfigValues(); // Show configuration values
                    break;
                case ConfigOptions.ResetConfig:
                    // Reset Configuration Values
                    var config = s_dal?.Config ?? throw new ConfigNotFoundException("s_dal.Config is null");
                    config.Reset();
                    Console.WriteLine("Configuration values reset.");
                    break;
                case ConfigOptions.Exit:
                    return;
                default:
                    throw new DalInvalidOperationException("Invalid option. Please try again.");

            }
        }
        while (selectedOption != ConfigOptions.Exit);
    }

    private static void SetRiskRange()
    {
        Console.WriteLine("Which value do you want to change?");
        Console.WriteLine("1. System Clock");
        Console.WriteLine("2. Risk Range");
        if (!int.TryParse(Console.ReadLine(), out int choice)) throw new DalFormatException("Invalid input for choice!");
        switch (choice)
        {
            case 1:
                Console.WriteLine("Enter new System Clock (yyyy-MM-dd HH:mm:ss): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime newClock)) throw new DalFormatException("Invalid date and time format!");
                s_dal.Config.Clock = newClock;
                Console.WriteLine($"New System Clock: {s_dal.Config.Clock}");
                break;
            case 2:
                Console.WriteLine("Enter new Risk Range (in minutes): ");
                if (!int.TryParse(Console.ReadLine(), out int minutes)) throw new DalFormatException("Invalid input for minutes!");
                s_dal.Config.RiskRange = TimeSpan.FromMinutes(minutes);
                Console.WriteLine($"New Risk Range: {s_dal.Config.RiskRange}");
                break;
            default:
                throw new DalInvalidOperationException("Invalid option. Please try again.");
        }
    }


    private static void ShowConfigValues()
    {
        Console.WriteLine("Which value do you want to change?");
        Console.WriteLine("1. System Clock");
        Console.WriteLine("2. Risk Range");
        if (!int.TryParse(Console.ReadLine(), out int choice)) throw new DalFormatException("Invalid input for choice!");
        switch (choice)
        {
            case 1:
                Console.WriteLine($"New System Clock: {s_dal.Config.Clock}");
                break;
            case 2:
                Console.WriteLine($"New Risk Range: {s_dal.Config.RiskRange}");
                break;
            default:
                throw new DalInvalidOperationException("Invalid option. Please try again.");

        }

    }


    private static void InitializeData()
    {
        Initialization.Do(s_dal);
        Console.WriteLine("Data initialized.");
    }


    private static void ShowAllData()
    {
        PrintVolunteer();
        PrintCall();
        PrintAssignment();


    }
    private static void PrintVolunteer()
    {
        List<Volunteer> newlist = s_dal.Volunteer.ReadAll().ToList();
        if (newlist != null && newlist.Count > 0)
        {
            foreach (var volunteer in newlist)
            {
                Console.WriteLine(volunteer);
            }
        }
        else
            Console.WriteLine("No volunteers in the database.");
    }

    private static void PrintCall()
    {
        List<Call> newlist = s_dal.Call.ReadAll().ToList();
        if (newlist != null && newlist.Count > 0)
        {
            foreach (var volunteer in newlist)
            {
                Console.WriteLine(volunteer);
            }
        }
        else
            Console.WriteLine("No calls in the database.");
    }
    private static void PrintAssignment()
    {
        List<Assignment> newlist = s_dal.Assignment.ReadAll().ToList();
        if (newlist != null && newlist.Count > 0)
        {
            foreach (var volunteer in newlist)
            {
                Console.WriteLine(volunteer);
            }
        }
        else
            Console.WriteLine("No Assignments in the database.");
    }
    

    private static void ResetDatabaseAndConfig()
    {
        s_dal!.Volunteer.DeleteAll();
        s_dal!.Call.DeleteAll();
        s_dal!.Assignment.DeleteAll();
        s_dal!.Config.Reset();
        Console.WriteLine("Database and configuration reset.");
    }

    private static void PrintEntitysMenu(string type)
    {

        Console.WriteLine($"{type} Sub-Menu:");
        Console.WriteLine("0. Exit Sub-Menu");
        Console.WriteLine($"1. Add New {type}");
        Console.WriteLine($"2. Read {type} ");
        Console.WriteLine($"3. Read All {type}s");
        Console.WriteLine($"4. Update {type} ");
        Console.WriteLine($"5. Delete {type} ");
        Console.WriteLine($"6. Delete All {type}s");
        Console.Write("Select an option: ");

    }

    //The function uses input from the user to create a new object of type Volunteer
    public static Volunteer InputVolunteer()
    {
        Console.WriteLine($"Creating new volunteer:");

        Console.WriteLine("Enter ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) throw new DalFormatException("Invalid ID!");

        Console.WriteLine("Enter Full Name: ");
        string fullName = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Phone (10 digits, starts with 0): ");
        string phone = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Email: ");

        string email = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Role (0 for Manager, 1 for Volunteer): ");
        if (!int.TryParse(Console.ReadLine(), out int roleInput)) throw new DalFormatException("Invalid Role!");
        MyRole role = (MyRole)roleInput;

        Console.WriteLine("Enter Type Distance (0 for Aerial, 1 for Walking, 2 for Driving): ");
        if (!int.TryParse(Console.ReadLine(), out int typeDistanceInput)) throw new DalFormatException("Invalid Type Distance!");
        MyTypeDistance typeDistance = (MyTypeDistance)typeDistanceInput;

        Console.WriteLine("Enter Password: ");
        string password = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Address: ");
        string address = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Latitude: ");
        if (!double.TryParse(Console.ReadLine(), out double latitude)) throw new DalFormatException("Invalid Latitude!");

        Console.WriteLine("Enter Longitude: ");
        if (!double.TryParse(Console.ReadLine(), out double longitude)) throw new DalFormatException("Invalid Longitude!");

        Console.WriteLine("Enter Max Distance: ");
        if (!double.TryParse(Console.ReadLine(), out double maxDistance)) throw new DalFormatException("Invalid Max Distance!");

        bool isActive = true;

        Volunteer newVolunteer = new Volunteer(id, fullName, phone, email, role, typeDistance, password, address, latitude, longitude, maxDistance, isActive);
        return newVolunteer;
    }

    // The function uses input from the user to create a new object of type Assignment

    public static Assignment InputAssignment()
    {
        Console.WriteLine("Creating new assignment:");

        Console.WriteLine("Enter Call ID: ");
        if (!int.TryParse(Console.ReadLine(), out int callId)) throw new DalFormatException("Invalid Call ID!");

        Console.WriteLine("Enter Volunteer ID: ");
        if (!int.TryParse(Console.ReadLine(), out int volunteerId)) throw new DalFormatException("Invalid Volunteer ID!");

        Console.WriteLine("Enter Start Call (YYYY-MM-DD HH:MM:SS): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime startCall)) throw new DalFormatException("Invalid Start Call!");

        Console.WriteLine("Enter Finish Type (optional, press Enter to skip): ");
        string finishTypeInput = Console.ReadLine() ?? "0";
        MyFinishType? finishType = string.IsNullOrEmpty(finishTypeInput) ? (MyFinishType?)null : Enum.Parse<MyFinishType>(finishTypeInput);

        Console.WriteLine("Enter Finish Call (optional, press Enter to skip, YYYY-MM-DD HH:MM:SS): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime finishCall)) throw new DalFormatException("Invalid Date");


        Assignment newAssignment = new Assignment(1, callId, volunteerId, startCall, finishType, finishCall);
        return newAssignment;
    }

    // The function uses input from the user to create a new object of type Call
    public static Call InputCall()
    {
        Console.WriteLine("Creating new call:");

        Console.WriteLine("Enter Call Type: ");
        if (!Enum.TryParse(Console.ReadLine(), out MyCallType callType)) throw new DalFormatException("Invalid Call Type!");

        Console.WriteLine("Enter Address: ");
        string address = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Latitude: ");
        if (!double.TryParse(Console.ReadLine(), out double latitude)) throw new DalFormatException("Invalid Latitude!");

        Console.WriteLine("Enter Longitude: ");
        if (!double.TryParse(Console.ReadLine(), out double longitude)) throw new DalFormatException("Invalid Longitude!");

        Console.WriteLine("Enter Open Time (YYYY-MM-DD HH:MM:SS): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime openTime)) throw new DalFormatException("Invalid Open Time!");

        Console.WriteLine("Enter Description (optional, press Enter to skip): ");
        string description = Console.ReadLine() ?? "0";

        Console.WriteLine("Enter Max Finish Call (optional, press Enter to skip, YYYY-MM-DD HH:MM:SS): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime maxFinishCallInput)) throw new DalFormatException("Invalid Date");



        Call newCall = new Call(1, callType, address, latitude, longitude, openTime, description, maxFinishCallInput);
        return newCall;
    }

    //The SubMenuConfig function displays a configuration submenu with options for advancing the system clock, showing or setting configuration values, and resetting configuration values
    public static void SubMenuConfig()
    {
        Console.WriteLine("Config Sub-Menu:");
        Console.WriteLine("0. Exit Sub-Menu");
        Console.WriteLine("1. Advance System Clock by a Minute");
        Console.WriteLine("2. Advance System Clock by an Hour");
        Console.WriteLine("3. Show Current System Clock");
        Console.WriteLine("4. Set New Configuration Value");
        Console.WriteLine("5. Show Current Configuration Values");
        Console.WriteLine("6. Reset Configuration Values");
        Console.Write("Select an option: ");
    }
    private static void VolunteerEntityMenu()
    {
        bool IsExit = false;
        do
        {
            PrintEntitysMenu("Volunteer");
            if (!int.TryParse(Console.ReadLine(), out int choiceInput)) throw new DalFormatException("Invalid input for menu option!");
            EntityMenuOption choice = (EntityMenuOption)choiceInput;
            try
            {
                switch (choice)
                {
                    case EntityMenuOption.Exit: ///0
                        {
                            IsExit = true;
                            break;
                        }
                    case EntityMenuOption.Create: //1
                        {
                            s_dal.Volunteer.Create(InputVolunteer());
                            Console.WriteLine("Volunteer created successfully!");
                            break;
                        }
                    case EntityMenuOption.Read: //2
                        {
                            Console.WriteLine("enter id: ");
                            if (!int.TryParse(Console.ReadLine(), out int id)) throw new DalFormatException("Invalid input for ID!");
                            Volunteer? volunteer = s_dal.Volunteer.Read(id);
                            if (volunteer == null)
                                throw new Exception($"Volunteer with ID={id} does not exists");
                            else
                                Console.WriteLine(volunteer); //print

                            break;
                        }
                    case EntityMenuOption.ReadAll: //3
                        {
                            PrintVolunteer();
                            break;
                        }
                    case EntityMenuOption.Update: //4
                        {
                            s_dal.Volunteer.Update(InputVolunteer());
                            Console.WriteLine("Volunteer updated successfully!");
                            break;
                        }
                    case EntityMenuOption.Delete: //5
                        {
                            Console.WriteLine("enter id: ");
                            if (!int.TryParse(Console.ReadLine(), out int id)) throw new DalFormatException("Invalid input for ID!");
                            s_dal.Volunteer.Delete(id);
                            Console.WriteLine($"Volunteer {id} deleted successfully!");
                            break;
                        }
                    case EntityMenuOption.DeleteAll: //6
                        {
                            s_dal.Volunteer.DeleteAll();
                            Console.WriteLine("Volunteers deleted successfully!");
                            break;
                        }
                    default:
                        {
                            throw new DalInvalidOperationException("Invalid choice");
                        }
                }
            }
            catch (Exception ExMsg)
            {
                Console.WriteLine(ExMsg);
                VolunteerEntityMenu();
            }

        } while (IsExit == false);//while not exit
    }
    private static void CallEntityMenu()
    {
        bool IsExit = false;
        do
        {
            PrintEntitysMenu("Call");
            if (!int.TryParse(Console.ReadLine(), out int choiceInput)) throw new DalFormatException("Invalid input for menu option!");
            EntityMenuOption choice = (EntityMenuOption)choiceInput;

            switch (choice)
            {
                case EntityMenuOption.Exit: //0
                    {
                        IsExit = true;
                        break;
                    }
                case EntityMenuOption.Create: //1
                    {
                        s_dal.Call.Create(InputCall());
                        Console.WriteLine("Call created successfully!");
                        break;
                    }
                case EntityMenuOption.Read: //2
                    {
                        Console.WriteLine("enter id: ");
                        if (!int.TryParse(Console.ReadLine(), out int id)) throw new DalFormatException("Invalid input for ID!");
                        Call? call = s_dal.Call.Read(id);
                        if (call == null)
                            throw new Exception($"Call with ID={id} does not exists");
                        else
                            Console.WriteLine(call); //print

                        break;
                    }
                case EntityMenuOption.ReadAll: //3
                    {
                        PrintCall();
                        break;
                    }
                case EntityMenuOption.Update: //4
                    {
                        s_dal.Call.Update(InputCall());
                        Console.WriteLine("Call updated successfully!");
                        break;
                    }
                case EntityMenuOption.Delete: //5
                    {
                        Console.WriteLine("enter id: ");
                        if (!int.TryParse(Console.ReadLine(), out int id)) throw new DalFormatException("Invalid input for ID!");
                        s_dal.Call.Delete(id);
                        Console.WriteLine($"Call {id} deleted successfully!");
                        break;
                    }
                case EntityMenuOption.DeleteAll: //6
                    {
                        s_dal.Call.DeleteAll();
                        Console.WriteLine("Calls deleted successfully!");
                        break;
                    }
                default:
                    {
                        throw new DalInvalidOperationException("Invalid choice");
                    }

            }

        } while (IsExit == false);//while not exit
    }
    private static void AssignmentEntityMenu()
    {
        bool IsExit = false;

        do
        {
            PrintEntitysMenu("Assignment");
            if (!int.TryParse(Console.ReadLine(), out int choiceInput)) throw new DalFormatException("Invalid input for menu option!");
            EntityMenuOption choice = (EntityMenuOption)choiceInput;

            switch (choice)
            {
                case EntityMenuOption.Exit: //0
                    {
                        IsExit = true;
                        break;
                    }
                case EntityMenuOption.Create: //1
                    {
                        s_dal.Assignment.Create(InputAssignment());
                        Console.WriteLine("Assignment created successfully!");
                        break;
                    }
                case EntityMenuOption.Read: //2
                    {
                        Console.WriteLine("enter id: ");
                        if (!int.TryParse(Console.ReadLine(), out int id)) throw new DalFormatException("Invalid input for ID!");
                        Assignment? assignment = s_dal.Assignment.Read(id);
                        if (assignment == null)
                            throw new Exception($"Assignment with ID={id} does not exists");
                        else
                            Console.WriteLine(assignment); //print
                        break;
                    }
                case EntityMenuOption.ReadAll: //3
                    {
                        PrintAssignment();
                        break;
                    }
                case EntityMenuOption.Update: //4
                    {
                        s_dal.Assignment.Update(InputAssignment());
                        Console.WriteLine("Assignment updated successfully!");
                        break;
                    }
                case EntityMenuOption.Delete: //5
                    {
                        Console.WriteLine("enter id: ");
                        if (!int.TryParse(Console.ReadLine(), out int id)) throw new DalFormatException("Invalid input for ID!");
                        s_dal.Assignment.Delete(id);
                        Console.WriteLine($"Assignment {id} deleted successfully!");
                        break;
                    }
                case EntityMenuOption.DeleteAll: //6
                    {
                        s_dal.Assignment.DeleteAll();
                        Console.WriteLine("Assignments deleted successfully!");
                        break;
                    }
                default:
                    {
                        throw new DalInvalidOperationException("Invalid choice");
                    }

            }

        } while (IsExit == false);//while not exit

    }
}

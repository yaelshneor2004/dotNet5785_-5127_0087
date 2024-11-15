using Dal;
using DalApi;

using DO;

namespace DalTest;
public enum MainMenuOptions { Exit, ShowVolunteer, ShowCall, ShowAssignment, InitializeData, ShowAllData, ShowConfigSubMenu, ResetDatabaseAndConfig }
public enum Crud
{
    Exit, Create, Read, ReadAll, Update, Delete, DeleteAll
}
internal class Program
{
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1
    public static void Main(string[] args)
    {
        try
        {
            MainMenuOptions selectedOption;
            do
            {
                ShowMainMenu();
                string st = Console.ReadLine()!;
                selectedOption = (MainMenuOptions)int.Parse(st);
                switch (selectedOption)
                {
                    case MainMenuOptions.ShowVolunteer:
                        ShowSubMenu("Volunteer",s_dalVolunteer);
                        break;
                    case MainMenuOptions.ShowCall:
                        ShowSubMenu("Call", s_dalCall);
                        break;
                    case MainMenuOptions.ShowAssignment:
                        ShowSubMenu("Assignment", s_dalAssignment);
                        break;
                    case MainMenuOptions.InitializeData:
                        InitializeData();
                        break;
                    case MainMenuOptions.ShowAllData:
                        ShowAllData();
                        break;
                        //
                    case MainMenuOptions.ShowConfigSubMenu:
                        ShowConfigSubMenu();
                        break;
                    case MainMenuOptions.ResetDatabaseAndConfig:
                        ResetDatabaseAndConfig();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }


            }
            while (selectedOption != 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    private static void ShowMainMenu()
    {
        Console.WriteLine("Main Menu:");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Displaying a submenu for the volunteers");
        Console.WriteLine("2. Show Entity 2 Sub-Menu");
        Console.WriteLine("3. Show Entity 3 Sub-Menu");
        Console.WriteLine("4. Initialize Data");
        Console.WriteLine("5. Show All Data");
        Console.WriteLine("6. Show Config Sub-Menu");
        Console.WriteLine("7. Reset Database and Config");
        Console.Write("Select an option: ");
    }

    private static void ShowVolunteers()
    {
        ShowSubMenu("Volunteer", s_dalVolunteer);
    }
    private static void ShowCall()
    { ShowSubMenu("Call", s_dalCall); }
    private static void ShowAssignment()
    {
        ShowSubMenu("Assignment", s_dalAssignment);
    }

    private static void ShowConfigSubMenu()
    {
          Crud selectedOption;
            do
            {
            SubMenuConfig();
                if (Enum.TryParse(Console.ReadLine(), out selectedOption))
                {
                    switch (selectedOption)
                    {
                        case Crud.Create:
                            // Advance System Clock by a Minute
                            s_dalConfig!.Clock = s_dalConfig.Clock.AddMinutes(1);
                            Console.WriteLine($"New System Clock: {s_dalConfig.Clock}");
                            break;
                        case Crud.Read:
                            // Advance System Clock by an Hour
                            s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
                            Console.WriteLine($"New System Clock: {s_dalConfig.Clock}");
                            break;
                        case Crud.ReadAll:
                            // Show Current System Clock
                            Console.WriteLine($"Current System Clock: {s_dalConfig.Clock}");
                            break;
                        case Crud.Update:
                            // Set New Configuration Value
                            Console.WriteLine("Enter New Risk Range (in minutes): ");
                            int minutes = int.Parse(Console.ReadLine()!);
                            s_dalConfig.RiskRange = TimeSpan.FromMinutes(minutes);
                            Console.WriteLine($"New Risk Range: {s_dalConfig.RiskRange}");
                            break;
                        case Crud.Delete:
                            // Show Current Configuration Values
                            Console.WriteLine($"Current System Clock: {s_dalConfig.Clock}");
                            Console.WriteLine($"Current Risk Range: {s_dalConfig.RiskRange}");
                            break;
                        case Crud.DeleteAll:
                            // Reset Configuration Values
                            s_dalConfig.Reset();
                            Console.WriteLine("Configuration values reset.");
                            break;
                        case Crud.Exit:
                            return;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid option number.");
                }
            } while (selectedOption != Crud.Exit);
        

    }
    private static void InitializeData()
    {
        Initialization.Do(s_dalVolunteer,s_dalAssignment, s_dalCall, s_dalConfig);
        Console.WriteLine("Data initialized.");
    }


    private static void ShowAllData()
    {
        ReadAllEntities("Volunteer", s_dalVolunteer);
        ReadAllEntities("Call", s_dalCall);
        ReadAllEntities("Assignment", s_dalAssignment);
    }


    private static void ResetDatabaseAndConfig()
    {
            s_dalVolunteer!.DeleteAll();
            s_dalCall!.DeleteAll();
            s_dalAssignment!.DeleteAll();
        s_dalConfig!.Reset();
        Console.WriteLine("Database and configuration reset.");

    }
    private static void ShowSubMenu(string type, Object dal)
    {
        ShowSubMenuOutput(type);
        string input = Console.ReadLine()!;
        Crud cr = (Crud)int.Parse(input);
        switch (cr)
        {
            case Crud.Create:
                CreateEntity(type, dal);
                break;
            case Crud.Read:
                ReadEntity(type, dal);
                break;
            case Crud.ReadAll:
                ReadAllEntities(type, dal);
                break;
            case Crud.Update:
                UpdateEntity(type, dal);
                break;
            case Crud.Delete:
                DeleteEntity(type, dal);
                break;
            case Crud.DeleteAll:
                ((dynamic)dal).DeleteAll();
                Console.WriteLine($"All {type}s deleted.");
                break;
            case Crud.Exit:
                return;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
    private static void ShowSubMenuOutput(string help)
    {

        Console.WriteLine($"{help} Sub-Menu:");
        Console.WriteLine("0. Exit Sub-Menu");
        Console.WriteLine($"1. Add New {help}");
        Console.WriteLine($"2. Read {help} ");
        Console.WriteLine($"3. Read All {help}s");
        Console.WriteLine($"4. Update {help} ");
        Console.WriteLine($"5. Delete {help} ");
        Console.WriteLine($"6. Delete All {help}s");
        Console.Write("Select an option: ");

    }
    private static void CreateEntity(string type, object dal)
    {
        dynamic entity;
        if (type == "Volunteer")
        {
            entity = inputV();
        }
        else if (type == "Call")
        {
            entity = inputC();
        }
        else
        {
            entity = inputA();
        }

        ((dynamic)dal).Create(entity);
        Console.WriteLine($"{type} created.");
    }

    private static void ReadEntity(string type, object dal)
    {
        Console.WriteLine($"Enter ID of the {type} to read:");
        int id = int.Parse(Console.ReadLine()!);
        var entity = ((dynamic)dal).Read(id);
        if (entity != null)
        {
            Console.WriteLine(entity.ToString());
        }
        else
            Console.WriteLine($"{type} not found.");
    }
    private static void ReadAllEntities(string type, object dal)
    {
        var entities = ((dynamic)dal).ReadAll();
        Console.WriteLine($"{type} List:");
        foreach (var entity in entities)
        {
            Console.WriteLine(entity);
        }
    }
    private static void DeleteEntity(string type, object dal)
    {
        Console.WriteLine($"Enter ID of the {type} to delete:");
        int.TryParse(Console.ReadLine(), out int id);
            var entity = ((dynamic)dal).Read(id);
            if (entity != null)
            {
                ((dynamic)dal).Delete(id);
                Console.WriteLine($"{type} deleted.");
            }
            else
            {
                Console.WriteLine($"{type} not found.");
            }
    }
    private static void UpdateEntity(string type, object dal)
    {
        dynamic entity;
        if (type == "Volunteer")
        {
            entity = inputV();
        }
        else if (type == "Call")
        {
            entity = inputC();
        }
        else
        {
            entity = inputA();
        }
        ((dynamic)dal).Update(entity);
        Console.WriteLine($"{type} updated.");

    }  
    public static Volunteer inputV()
    {
        Console.WriteLine($"Creating new volunteer:");

        Console.WriteLine("Enter ID: ");
        int id = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter Full Name: ");
        string fullName = Console.ReadLine()!;

        Console.WriteLine("Enter Phone (10 digits, starts with 0): ");
        string phone = Console.ReadLine()!;

        Console.WriteLine("Enter Email: ");
        string email = Console.ReadLine()!;

        Console.WriteLine("Enter Role (0 for Manager, 1 for Volunteer): ");
        MyRole role = (MyRole)int.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter Type Distance (0 for Aerial, 1 for Walking, 2 for Driving): ");
        MyTypeDistance typeDistance = (MyTypeDistance)int.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter Password: ");
        string password = Console.ReadLine()!;

        Console.WriteLine("Enter Address: ");
        string address = Console.ReadLine()!;

        Console.WriteLine("Enter Latitude: ");
        double latitude = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter Longitude: ");
        double longitude = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter Max Distance: ");
        double maxDistance = double.Parse(Console.ReadLine()!);

        bool isActive = true;

        Volunteer newVolunteer = new Volunteer(id, fullName, phone, email, role, typeDistance, password, address, latitude, longitude, maxDistance, isActive);
        return newVolunteer;
    }



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

}

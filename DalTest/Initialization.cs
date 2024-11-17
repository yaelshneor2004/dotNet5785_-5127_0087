

namespace DalTest;
using DalApi;
using DO;
public static class Initialization
{
    private static IAssignment? s_dalAssignment; //stage 1
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();
    public static bool CheckID(int idNumber)
    {
        string idStr = idNumber.ToString("D9");
        if (idStr.Length != 9)
        {
            return false;
        }

        int sum = 0;
        for (int i = 0; i < idStr.Length; i++)
        {
            int num = (idStr[i] - '0') * ((i % 2) + 1);
            if (num > 9)
            {
                num -= 9;
            }
            sum += num;
        }

        return (sum % 10) == 0;
    }




private static void createVolunteer()
    {
        string[] volunteerNames =
       { "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein", "Shira Israelof", "David Ben-Gurion", "Golda Meir", "Yitzhak Rabin", "Chaim Weizmann", "Menachem Begin", "Ariel Sharon", "Moshe Dayan", "Shimon Peres", "Yigal Alon" };
        string[] addresses = { "Rothschild 10, Tel Aviv, Israel", "Yigal Alon 120, Tel Aviv, Israel", "Ibn Gvirol 30, Tel Aviv, Israel", "Derech Hevron 78, Jerusalem, Israel", "Herzl Blvd 103, Jerusalem, Israel", "Jaffa St 45, Jerusalem, Israel", "Herzl 9, Haifa, Israel", "Moriah Blvd 70, Haifa, Israel", "The Prophets 11, Haifa, Israel", "Rav Shauli 5, Be'er Sheva, Israel", "Reger Blvd 55, Be'er Sheva, Israel", "Johanna Jabotinsky 15, Be'er Sheva, Israel", "Begin Way 234, Petah Tikva, Israel", "Weizmann 22, Petah Tikva, Israel", "Eli Cohen 1, Ramat Gan, Israel" };
            Random rand = new Random();
       for (int i=0;i<15;i++)
        {
            string name=volunteerNames[i];
            string address=addresses[i];
            string phone = "05" + rand.Next(0, 10) + rand.Next(10000000, 99999999).ToString();
            string email = $"{name.Replace(" ", "").ToLower()}@gmail.com";
            double? maxDistance = (rand.Next(2) == 0) ? (rand.NextDouble() * 95 + 5) : (double?)null;
            int id;
            do
                id = s_rand.Next(200000000, 400000000);
            while (s_dalVolunteer!.Read(id) != null&& CheckID(id));

 
            s_dalVolunteer!.Create(new(id, name, phone, email,0,0, ,address, , ,maxDistance,true));
        }



    }
    private static void createsAssignment()
    { }
    private static void createsCall()
    {

    
        DateTime start = new DateTime(s_dalConfig.Clock.Year - 2, 1, 1); //stage 1
        int range = (s_dalConfig.Clock - start).Days; //stage 1
         start.AddDays(s_rand.Next(range));
    }

    public static void Do(IVolunteer? dalVolunteer, IAssignment? dalAssignment, ICall? dalCall, IConfig? dalConfig) //stage 1
    {
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1                                                                                            //...
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset(); //stage 1
        s_dalCall.DeleteAll(); //stage 1
        s_dalAssignment.DeleteAll(); //stage 1
        s_dalVolunteer.DeleteAll(); //stage 1
        Console.WriteLine("Initializing Volunteer list ...");
        createVolunteer();
        createsCall();
        createsAssignment();
    }
}


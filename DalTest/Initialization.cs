

namespace DalTest;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System;

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
        string[] addresses =  { "Rothschild 10, Tel Aviv, Israel", "Yigal Alon 120, Tel Aviv, Israel", "Ibn Gvirol 30, Tel Aviv, Israel",
    "Derech Hevron 78, Jerusalem, Israel", "Herzl Blvd 103, Jerusalem, Israel", "Jaffa St 45, Jerusalem, Israel",
    "Herzl 9, Haifa, Israel", "Moriah Blvd 70, Haifa, Israel", "The Prophets 11, Haifa, Israel",
    "Rav Shauli 5, Be'er Sheva, Israel", "Reger Blvd 55, Be'er Sheva, Israel", "Johanna Jabotinsky 15, Be'er Sheva, Israel",
    "Begin Way 234, Petah Tikva, Israel", "Weizmann 22, Petah Tikva, Israel", "Eli Cohen 1, Ramat Gan, Israel" };


        Random rand = new Random();
       for (int i=0;i<15;i++)
        {
            double? latitude = null;
            double? longitude = null;
            string password = $"{new Random().Next(100000, 999999)}{(char)new Random().Next('A', 'Z' + 1)}{(char)new Random().Next('a', 'z' + 1)}";
            string name=volunteerNames[i];
            string address=addresses[i];
            string phone = "05" + rand.Next(0, 10) + rand.Next(10000000, 99999999).ToString();
            string email = $"{name.Replace(" ", "").ToLower()}@gmail.com"; 
            double? maxDistance = (rand.Next(2) == 0) ? (rand.NextDouble() * 95 + 5) : (double?)null;
            int id;
            do
                id = s_rand.Next(200000000, 400000000);
            while (s_dalVolunteer!.Read(id) != null); // id

            //create 15 Volunteer
            s_dalVolunteer!.Create(new(id, name, phone, email, MyRole.Volunteer, MyTypeDistance.Aerial, password ,address, latitude, longitude, maxDistance,true));
        }

       //create 2 managers
        s_dalVolunteer!.Create(new(327770087, "Ayala Ozeri", "0533328200", "ayala.ozeri@gmail.com", MyRole.Manager, MyTypeDistance.Aerial, "ayala19", "Rothschild 10, Tel Aviv, Israel", 32.0625, 34.7721, 10.0, true));
        s_dalVolunteer!.Create(new(326615127, "Yael Shneor", "0533859299", "y7697086@gmail.com", MyRole.Manager, MyTypeDistance.Aerial, "yaelS2208", "Derech Hevron 78, Jerusalem, Israel", 31.7525, 35.2121, 20.0, true));



    }
    private static void createsAssignment()
    {
        Random rand = new Random();

        var allCalls = s_dalCall!.ReadAll(); // שליפת כל הקריאות
        var allVolunteers = s_dalVolunteer!.ReadAll(); // שליפת כל המתנדבים
        int idIndex = 1;

        foreach (var call in allCalls)
        {
            var volunteer = allVolunteers[rand.Next(allVolunteers.Count)]; // random Volunteers
            DateTime startCall = call.OpenTime.AddHours(rand.Next(1, 12)); // Start between 1 hour and 12 hours after opening//לבדוק מה הדרישות בפרוייקט

            // סוג סיום ותאריך סיום אופציונליים
            //לבדוק את זה-מה הדרישות בפרוייקט
            MyFinishType? finishType = rand.Next(2) == 0 ? (MyFinishType?)MyFinishType.Treated : null; //finishType can be null if the convert not success
            DateTime? finishCall = finishType != null ? startCall.AddHours(rand.Next(1, 24)) : null;

            s_dalAssignment!.Create(new Assignment(idIndex, , volunteer.Id,startCall,finishType,finishCall));


        }
    }
    private static void createsCall()
    {
        Random rand = new Random();

        string[] callAddresses =  {"10 Jaffa Street, Jerusalem",
    "20 Emek Refaim Street, Jerusalem",
    "30 Agripas Street, Jerusalem",
    "40 Shmuel HaNavi Street, Jerusalem",
    "50 HaNevi'im Street, Jerusalem",
    "60 Gaza Street, Jerusalem",
    "4 Herzl Boulevard, Jerusalem",
    "40 Masliansky Street, Jerusalem",
    "10 Ben Yehuda Street, Jerusalem",
    "150 Hebron Road, Jerusalem",
    "5 Hillel Street, Jerusalem",
    "6 Korazin Street, Jerusalem",
    "35 King George Street, Jerusalem",
    "78 Ben Zvi Street, Jerusalem",
    "4 Devorah HaNeviah Street, Jerusalem",
    "15 Yehuda HaNasi Street, Jerusalem",
    "5 Aliash Street, Jerusalem",
    "15 Arlozorov Street, Jerusalem",
    "13 Shamai Street, Jerusalem",
    "10 Tavor Street, Jerusalem",
    "55 Ussishkin Street, Jerusalem",
    "7 Helene HaMalka Street, Jerusalem",
    "14 Lincoln Street, Jerusalem",
    "40 Zerach Street, Jerusalem",
    "22 Mea Shearim Street, Jerusalem",
    "10 King David Street, Jerusalem",
    "30 Ruth Street, Jerusalem",
    "9 Beit HaDfus Street, Jerusalem",
    "5 David Yellin Street, Jerusalem",
    "8 Shmuel HaNagid Street, Jerusalem",  
    "15 HaTzanhanim Street, Jerusalem",
    "9 Rabbi Akiva Street, Jerusalem",
    "7 Greenberg Street, Jerusalem",
    "6 Henrietta Szold Street, Jerusalem",
    "5 Shmuel HaNagid Street, Jerusalem",
    "18 Beit Vagan Street, Jerusalem",
    "12 Neve Sha'anan Street, Jerusalem",
    "3 Rachel Imenu Street, Jerusalem",
    "15 Hillel Street, Jerusalem",
    "8 Bezalel Street, Jerusalem",
    "9 Kiryat Moshe Street, Jerusalem",
    "13 Pierre Koenig Street, Jerusalem",
    "24 Rabbi Herzog Street, Jerusalem",
    "5 Ein Gedi Street, Jerusalem",
    "8 HaOman Street, Jerusalem",
    "5 Ibn Shaprut Street, Jerusalem",
    "25 Bar Ilan Street, Jerusalem",
    "6 Yitzhak Kariv Street, Jerusalem",
    "17 Weizmann Street, Jerusalem",
    "9 Shmuel HaNagid Street, Jerusalem",
    "7 Ben Ze'ev Street, Jerusalem",
    "10 Rivlin Street, Jerusalem"
};

        double[] callLongitudes = new double[]  { 31.78130811991647, 31.76543961779154, 31.783470280009738,31.791048846509597,31.78391107912798, 31.771053788166768, 31.785777797665002, 31.817876781912197, 31.78186936361579, 31.74578724034863, 31.78068910546481, 31.78255160178496, 31.779769, 31.7798566, 31.789588, 31.755770, 31.755974, 31.781584, 31.772610, 31.781239, 31.782361, 31.780486, 31.7820255, 31.775421, 31.816719, 31.787706, 31.776943, 31.7765005, 31.785952, 31.786426, 31.780594, 31.779808, 31.816370, 31.779226, 31.817092, 31.757365, 31.780274, 31.771251, 31.771493, 31.763331, 31.780627, 31.780728, 31.786061, 31.756843, 31.769767, 31.753902, 31.748810, 31.776448, 31.794707, 31.777475, 31.790191, 31.779852, 31.780946 };

        double[] callLatitudes = new double[] { 35.22119546257982, 35.22123577032626,  35.215657362641764,  35.224665335668945, 35.221665622205705, 35.21205714918638, 35.19764122028971, 35.18968763754008, 35.21774235102746, 35.21658163568471, 35.21638617800225, 35.209312878017535, 35.2159379, 35.2092867, 35.224894, 35.202146, 35.201317, 35.215236, 35.216455, 35.218053, 35.213336, 35.212081, 35.221056, 35.219964, 35.190149, 35.222337, 35.222155, 35.217356, 35.189180, 35.216914, 35.214816, 35.226296, 35.192176, 35.218002, 35.191500, 35.160694, 35.214955, 35.183917, 35.200909, 35.218213, 35.217068, 35.214544, 35.195686, 35.215021, 35.208627, 35.221349, 35.211373, 35.211675, 35.218078, 35.224944, 35.198722, 35.214792, 35.220409};

        int idIndex = 1;//Not really consumed, getting booted from CallImplementation
        for (int i = 0; i < callAddresses.Length; i++)
        {
            string address = callAddresses[i];
            MyCallType callType = 0;
            double latitude = callLatitudes[idIndex];
            double longitude = callLongitudes[idIndex];
            DateTime openTime = new DateTime(s_dalConfig.Clock.Year - 2, 1, 1); //stage 1
            int range = (s_dalConfig.Clock - openTime).Days; //stage 1
            openTime.AddDays(s_rand.Next(range));

            DateTime maxFinishCall = new DateTime(s_dalConfig.Clock.Year +1, 6, 1); //stage 1
            int range2 = (s_dalConfig.Clock - maxFinishCall).Days; //stage 1
            maxFinishCall.AddDays(s_rand.Next(range));


            string? description = $" call number: {idIndex} of type: {callType} at: {address}";

            if (i < 5)
            {
                maxFinishCall = s_dalConfig.Clock.AddDays(-rand.Next(1, 30)); //30 days before
                description = $"Call number: {i + 1} of type: {callType} at: {callAddresses[i]}";
            }
            
            s_dalCall!.Create(new Call(idIndex, callType, address, latitude, longitude, openTime, description, maxFinishCall));
        }
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


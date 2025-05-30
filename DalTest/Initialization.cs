﻿namespace DalTest;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System;
using System.Security.Cryptography;
using System.Text;

public static class Initialization
{
    private static IDal? s_dal; 
    private static readonly Random s_rand = new(); // Random number generator
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789ABCDEF"); // 16-byte key
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("ABCDEF9876543210");  // 16-byte initialization vector


    /// <summary>
    /// The createsAssignment method allocates volunteers to 50 calls from a list, calculates start and end times, assigns a random finish type, and creates new assignments
    /// </summary>
    private static void createVolunteer()
    {
   
        string[] volunteerNames =
        {
        "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein", "Shira Israelof", "David BenGurion",
        "Golda Meir", "Yitzhak Rabin", "Chaim Weizmann", "Menachem Begin", "Ariel Sharon", "Moshe Dayan", "Shimon Peres", "Yigal Alon"
    };
        string[] addresses =
        {
        "Rothschild 10, Tel Aviv, Israel", "Yigal Alon 120, Tel Aviv, Israel", "Ibn Gvirol 30, Tel Aviv, Israel",
        "Derech Hevron 78, Jerusalem, Israel", "Herzl Blvd 103, Jerusalem, Israel", "Jaffa St 45, Jerusalem, Israel",
        "Herzl 9, Haifa, Israel", "Moriah Blvd 70, Haifa, Israel", "The Prophets 11, Haifa, Israel",
        "Rav Shauli 5, Be'er Sheva, Israel", "Reger Blvd 55, Be'er Sheva, Israel", "Johanna Jabotinsky 15, Be'er Sheva, Israel",
        "Begin Way 234, Petah Tikva, Israel", "Weizmann 22, Petah Tikva, Israel", "Eli Cohen 1, Ramat Gan, Israel"
    };

        double[] Longitudes = new double[]
        {
        34.77697, 34.79152, 34.78139, 35.21289, 35.18331, 35.22323, 34.99115, 34.98264, 34.98865, 34.79223, 34.77952,
        34.78251, 34.88718, 34.88532, 34.82732
        };
        double[] Latitudes = new double[]
        {
        32.06366, 32.06180, 32.08404, 31.74851, 31.76832, 31.78571, 32.79916, 32.79406, 32.81912, 31.25297, 31.24642,
        31.25360, 32.09077, 32.08862, 32.08333
        };
        int[] validIds = new int[] {
    967530684, 469333751, 128044765, 484501226, 975856709, 747695906,
 975363193, 674550785, 824564579, 680710332, 301150447, 190313205,
 781021860, 700156714, 721016087, 142161512, 698811734, 627262876,
 603051947, 421137647
 };
        for (int i = 0; i < 15; i++)
        {
            double? latitude = Latitudes[i]; // Latitude of the call at index i
            double? longitude = Longitudes[i]; // Longitude of the call at index i
            string password = $"{new Random().Next(100000, 999999)}{(char)new Random().Next('A', 'Z' + 1)}{(char)new Random().Next('a', 'z' + 1)}"; // Generate random password with numbers and letters
            password += "!";
            password = Encrypt(password);
            string name = volunteerNames[i]; // Name of the volunteer at index i
            string address = addresses[i]; // Address from the array at index i
            string phone = "05" + s_rand.Next(10) + s_rand.Next(10000000, 100000000); // Generate random phone number
            string email = $"{name.Replace(" ", "").ToLower()}@gmail.com"; // Construct email using name, removing spaces and converting to lowercase
           int typeD = s_rand.Next(0, 3);
            double? maxDistance = s_rand.NextDouble() * 90 + 10;  // Generate max distance or set to null
            int idV=validIds[i];
            s_dal?.Volunteer.Create(new(idV, name, phone, email, MyRole.Volunteer,(DO.MyTypeDistance)typeD, password, address, latitude, longitude, maxDistance, true));
        }
        string ayalaPassword = Encrypt("Ayala19!");
          string yaelPassword = Encrypt("yaelS2208!");
        string yosefPassword = Encrypt("yosefL106!");
        // Create 2 managers
        s_dal?.Volunteer.Create(new(327770087, "Ayala Ozeri", "0533328200", "ayala.ozeri@gmail.com", MyRole.Manager, MyTypeDistance.Aerial, ayalaPassword, "Rothschild 10, Tel Aviv, Israel", 32.0625, 34.7721, 10.0, true));
            s_dal?.Volunteer.Create(new(326615127, "Yael Shneor", "0533859299", "y7697086@gmail.com", MyRole.Manager, MyTypeDistance.Traveling, yaelPassword, "Derech Hevron 78, Jerusalem, Israel", 31.7525, 35.2121, 20.0, true));
        s_dal?.Volunteer.Create(new(208799387, "Yosef Levi", "0527170106", "106mimi@gmail.com", MyRole.Volunteer, MyTypeDistance.Traveling, yosefPassword, "10 Jaffa Street, Jerusalem", 31.78130811991647, 35.22119546257982, 30.0, true));
    }
    /// <summary>
    /// The createsAssignment method reads volunteers and calls, then allocates volunteers to 50 calls, calculates start and finish times, assigns random finish types, and creates new assignments. 🌟
    /// </summary>
    private static void createsAssignment()
    { 
        int count = 0;
        var volunteers = s_dal?.Volunteer.ReadAll();
        var calls = s_dal?.Call.ReadAll()?.Where(call => call != null).Take(50)?.ToList();
        int volunteerId;
        var volunteerList = volunteers?.ToList();
        foreach (var call in calls ?? Enumerable.Empty<Call>())
        {
            count++;
            int callId = call.Id;
            do
            {
                volunteerId = volunteerList != null && volunteerList.Count > 0
               ? volunteerList[s_rand.Next(volunteerList.Count)].Id : 0;
            }
            while (volunteers != null && !volunteers.Any(v => v.Id == volunteerId)) ;
            DateTime startCall = call.OpenTime.AddHours(s_rand.Next(0, (int)(call.OpenTime.AddDays(21) - call.OpenTime).TotalHours) + 1);
            DateTime? finishCall;
            MyFinishType? finishType;
            if (count>10&&count<35)
            {
                finishCall = null;
                finishType = null;
            }
            else
            {
                finishCall = startCall.AddDays(s_rand.Next(1, 54));
                finishType = (MyFinishType)s_rand.Next(0, 4);
       }
            s_dal?.Assignment.Create(new Assignment(0, callId, volunteerId, startCall, finishType, finishCall));
        }

    }


    /// <summary>
    /// The createsCall method generates call data, including addresses, coordinates, open and max finish times, and creates new call entries
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private static void createsCall()
    { 
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
    "10 Rivlin Street, Jerusalem",
    "120 Ibn Gabirol St, Tel Aviv",
    "13 Ben Gurion Blvd, Holon",
    "45 Jerusalem St, Ramat Gan",
    "200 Jabotinsky St, Petah Tikva",
    "30 Herzl Blvd, Rishon LeZion",
    "15 Rothschild St, Netanya",
    "88 HaNesi'im St, Be'er Sheva",
    "100 Yitzhak Rabin Blvd, Haifa",
    "9 HaGefen St, Kfar Saba",
    "70 Weizmann St, Herzliya",
    "22 HaBanim St, Givatayim",
    "55 Allenby St, Tel Aviv",
    "Rothschild 10, Tel Aviv, Israel",
    "10 HaYarkon St, Eilat",
    "18 Sokolov St, Ramat HaSharon"};
        
        double[] callLatitudes = new double[]  { 31.78130811991647, 31.76543961779154, 31.783470280009738,31.791048846509597,31.78391107912798, 31.771053788166768, 31.785777797665002, 31.817876781912197, 31.78186936361579, 31.74578724034863, 31.78068910546481, 31.78255160178496, 31.779769, 31.7798566, 31.789588, 31.755770, 31.755974, 31.781584, 31.772610, 31.781239, 31.782361, 31.780486, 31.7820255, 31.775421, 31.816719, 31.787706, 31.776943, 31.7765005, 31.785952, 31.786426, 31.780594, 31.779808, 31.816370, 31.779226, 31.817092, 31.757365, 31.780274, 31.771251, 31.771493, 31.763331, 31.780627, 31.780728, 31.786061, 31.756843, 31.769767, 31.753902, 31.748810, 31.776448, 31.794707, 31.777475, 31.790191, 31.779852, 31.780946 , 32.0853, 32.0142, 32.0833, 32.0922, 31.9674, 32.3295, 31.2518, 32.7940, 32.1782, 32.1658, 32.0703, 32.0644, 31.8014, 29.5535, 32.1465 };

        double[] callLongitudes = new double[] { 35.22119546257982, 35.22123577032626,  35.215657362641764,  35.224665335668945, 35.221665622205705, 35.21205714918638, 35.19764122028971, 35.18968763754008, 35.21774235102746, 35.21658163568471, 35.21638617800225, 35.209312878017535, 35.2159379, 35.2092867, 35.224894, 35.202146, 35.201317, 35.215236, 35.216455, 35.218053, 35.213336, 35.212081, 35.221056, 35.219964, 35.190149, 35.222337, 35.222155, 35.217356, 35.189180, 35.216914, 35.214816, 35.226296, 35.192176, 35.218002, 35.191500, 35.160694, 35.214955, 35.183917, 35.200909, 35.218213, 35.217068, 35.214544, 35.195686, 35.215021, 35.208627, 35.221349, 35.211373, 35.211675, 35.218078, 35.224944, 35.198722, 35.214792, 35.220409, 34.7818, 34.7748, 34.8101, 34.8670, 34.7996, 34.8532, 34.7915, 34.9896, 34.9076, 34.8352, 34.8114, 34.7722, 34.6553, 34.9519, 34.8392 };

        int idIndex = 1;//just for sending
        for (int i = 0; i < callAddresses.Length; i++)
        {
            string address = callAddresses[i];
            MyCallType callType =(MyCallType) s_rand.Next(0,4);
            double latitude = callLatitudes[i];
            double longitude = callLongitudes[i];
            DateTime openTime = DateTime.Now.AddYears(-1).AddDays(s_rand.Next((DateTime.Now -DateTime.Now.AddYears(-1)).Days));
            DateTime maxFinishCall = openTime.AddDays(s_rand.Next(1, 50)); 
            string? description = $"call number: {idIndex} of type: {callType} at: {address}";
            if (5 < i &&i< 35)
            {
                 openTime = DateTime.Now.AddDays(-s_rand.Next(0, 21));
                 maxFinishCall = openTime.AddDays(s_rand.Next(1, 60));
            }
            if(i>50&&i<65)
            {
                openTime = DateTime.Now.AddDays(-s_rand.Next(0, 6));
                maxFinishCall = openTime.AddDays(s_rand.Next(1, 60));
            }
            if (i < 5)
            {
                if (s_dal?.Config != null)
                {
                    maxFinishCall = s_dal.Config.Clock.AddDays(-s_rand.Next(1, 30));
                }
                else
                {
                    throw new InvalidOperationException("Configuration is not initialized.");
                }
                   maxFinishCall = s_dal.Config.Clock.AddDays(-s_rand.Next(1, 30)); // 30 days before
                description = $"Call number: {i + 1} of type: {callType} at: {callAddresses[i]}";
            }

            s_dal!.Call.Create(new Call(idIndex, callType, address, latitude, longitude, openTime, description, maxFinishCall));
        }
    }

    //initializes and validates DAL objects, resets configurations, deletes existing data, and then initializes volunteers, calls, and assignments
    public static void Do() 
    {
        s_dal = DalApi.Factory.Get;
        s_dal.ResetDB();
        createVolunteer();
        createsCall();
        createsAssignment();
        Console.WriteLine("Data initialized successfully!");
    }
    private static string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return Convert.ToBase64String(encryptedBytes);
        }
    }
}


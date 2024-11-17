

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
        string[] addresses = { "Rothschild 10, Tel Aviv, Israel", "Yigal Alon 120, Tel Aviv, Israel", "Ibn Gvirol 30, Tel Aviv, Israel", "Derech Hevron 78, Jerusalem, Israel", "Herzl Blvd 103, Jerusalem, Israel", "Jaffa St 45, Jerusalem, Israel", "Herzl 9, Haifa, Israel", "Moriah Blvd 70, Haifa, Israel", "The Prophets 11, Haifa, Israel", "Rav Shauli 5, Be'er Sheva, Israel", "Reger Blvd 55, Be'er Sheva, Israel", "Johanna Jabotinsky 15, Be'er Sheva, Israel", "Begin Way 234, Petah Tikva, Israel", "Weizmann 22, Petah Tikva, Israel", "Eli Cohen 1, Ramat Gan, Israel" };
        double[] Latitude = { 3.2,5.4 };
        double[] Longitude = { 7.2,1.5 };
        Random rand = new Random();
       for (int i=0;i<15;i++)
        {
            string name=volunteerNames[i];
            string address=addresses[i];
            double latitude = Latitude[i];
            double longitude = Longitude[i];
            string phone = "05" + rand.Next(0, 10) + rand.Next(10000000, 99999999).ToString();
            string email = $"{name.Replace(" ", "").ToLower()}@gmail.com";
            double? maxDistance = (rand.Next(2) == 0) ? (rand.NextDouble() * 95 + 5) : (double?)null;
            int id;
            do
                id = s_rand.Next(200000000, 400000000);
            while (s_dalVolunteer!.Read(id) != null&& CheckID(id));

 
            s_dalVolunteer!.Create(new(id, name, phone, email,0,0, ,address, latitude, longitude, maxDistance,true));
        }



    }
    private static void createsAssignment()
    {
        Random rand = new Random();

        var allCalls = s_dalCall!.ReadAll(); // שליפת כל הקריאות
        var allVolunteers = s_dalVolunteer!.ReadAll(); // שליפת כל המתנדבים
        int idIndex = 0;

        foreach (var call in allCalls)
        {
            var volunteer = allVolunteers[rand.Next(allVolunteers.Count)]; // random Volunteers
            DateTime startCall = call.OpenTime.AddHours(rand.Next(1, 12)); // Start between 1 hour and 12 hours after opening//לבדוק מה הדרישות בפרוייקט

            // סוג סיום ותאריך סיום אופציונליים
            //לבדוק את זה-מה הדרישות בפרוייקט
            MyFinishType? finishType = rand.Next(2) == 0 ? (MyFinishType?)MyFinishType.Treated : null; //finishType can be null if the convert not success
            DateTime? finishCall = finishType != null ? startCall.AddHours(rand.Next(1, 24)) : null;

            s_dalAssignment!.Create(new Assignment(idIndex++, call.Id,volunteer.Id,startCall,finishType,finishCall));


        }
    } 
    private static void createsCall()
    {
        Random rand = new Random();

        string[] callAddresses = { /* רשימה של 50 כתובות */ };
        double[] callLatitudes = { /* 50 קווי רוחב */ };
        double[] callLongitudes = { /* 50 קווי אורך */ };

        int idIndex = 0;
        foreach (var address in callAddresses)
        {
            double latitude = callLatitudes[idIndex];
            double longitude = callLongitudes[idIndex];
            DateTime openTime = DateTime.Now.AddDays(-rand.Next(0, 30)); // up to 30 days back
            string? description = $" call number: {idIndex} of type: {callType} at: {address}";
            DateTime? maxFinishCall = rand.Next(2) == 0 ? openTime.AddHours(rand.Next(1, 72)) : null; //Will randomly accept any date and time between 1 hour and 72 hours after openTime or be null. 🌟

            DateTime start = new DateTime(s_dalConfig.Clock.Year - 2, 1, 1); //stage 1
            int range = (s_dalConfig.Clock - start).Days; //stage 1
            start.AddDays(s_rand.Next(range));


            s_dalCall!.Create(new Call(idIndex++, callType, address, latitude, longitude, openTime, description, maxFinishCall);
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


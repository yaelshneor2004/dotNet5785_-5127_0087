

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
    

private static void createVolunteer()
    {
        string[] volunteerNames =
       { "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein", "Shira Israelof", };

        foreach (var name in studentNames)
        {
            int id;
            do
                id = s_rand.Next(MIN_ID, MAX_ID);
            while (s_dalStudent!.Read(id) != null);

            bool? even = (id % 2) == 0 ? true : false;
            string? alias = even ? name + "ALIAS" : null;
            DateTime start = new DateTime(1995, 1, 1);
            DateTime bdt = start.AddDays(s_rand.Next((s_dalConfig.Clock - start).Days));

            s_dalStudent!.Create(new(id, name, alias, even, bdt));
        }

    }
}

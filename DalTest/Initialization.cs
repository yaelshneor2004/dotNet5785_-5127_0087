

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
    //...
}

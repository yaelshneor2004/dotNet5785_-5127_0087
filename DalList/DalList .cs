namespace Dal;
    using DalApi;
using DO;
sealed internal class DalList : IDal
{
    private DalList() { }
    public static IDal Instance => Nested.instance;
    private static class Nested
    {
        internal static readonly DalList instance = new DalList();
    }
    public IAssignment Assignment { get; } = new AssignmentImplementation();


    public ICall Call { get; } = new CallImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }
}

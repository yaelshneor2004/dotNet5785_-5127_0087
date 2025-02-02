
using DalApi;

namespace Dal;

sealed internal class DalXml : IDal
{    //full lazy Singleton
    private DalXml() { }
    //thread safe and single instance   
    public static IDal Instance => Nested.instance;
        private static class Nested
        {
            internal static readonly DalXml instance = new DalXml();
        }
        public IConfig Config { get; } = new ConfigImplementation();

    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}

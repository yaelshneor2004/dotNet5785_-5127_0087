

namespace Dal;
    using DalApi;
using DO;

sealed public class DalList : IDal
{
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

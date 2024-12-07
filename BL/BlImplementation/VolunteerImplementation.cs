using BlApi;
using System.Linq;
namespace BlImplementation;



internal class VolunteerImplementation:IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public BO.MyRole Login(string username, string password)
    {
        // Retrieve the list of volunteers from the DAL
        var volunteers = _dal.Volunteer.ReadAll();

        // Search for the user by username
        var user = volunteers.FirstOrDefault(v => v.FullName == username);

        // Check if the user exists
        if (user == null)
        {
            throw new UnauthorizedAccessException("Username or password is incorrect.");
        }

        // Check if the password is correct
        if (user.Password != password)
        {
            throw new UnauthorizedAccessException("Username or password is incorrect.");
        }

        // Return the user's role
        return(BO.MyRole)user.Role;
    }

    public void AddVolunteer(BO.Volunteer myVolunteer)
    {
        throw new NotImplementedException();
    }

    public void DeleteVolunteer(int id)
    {
        throw new NotImplementedException();
    }

    public BO.Volunteer GetVolunteerDetails(int id)
    {
        throw new NotImplementedException();
    }
    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? isActive, BO.MyCurrentCallType? myCurrentCallType)
    {
        var volunteers = _dal.Volunteer.ReadAll();

        // Filter by the IsActive status value
        if (isActive.HasValue)
            volunteers = volunteers.Where(v => v.IsActive == isActive.Value);


        // Mapping the results to BO.VolunteerInList
        var filteredVolunteers = volunteers.Select(v => new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            IsActive = v.IsActive,
            TotalCallsHandled = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id && a.FinishType == DO.MyFinishType.Treated).Count(),
            TotalCallsCancelled = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id && (a.FinishType == DO.MyFinishType.SelfCancel || a.FinishType == DO.MyFinishType.ManagerCancel)).Count(),
            TotalCallsExpired = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id && a.FinishType == DO.MyFinishType.ExpiredCancel).Count(),
            CurrentCallId = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id && a.FinishCall == null).Select(a => (int?)a.CallId).FirstOrDefault(),
            CurrentCallType = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id && a.FinishCall == null)
            .Select(a => _dal.Call.ReadAll(c => c.Id == a.CallId).Select(c => (BO.MyCurrentCallType?)c.CallType).FirstOrDefault()).FirstOrDefault() ?? BO.MyCurrentCallType.None
        }).ToList();

        // Sort by the defined field value or by Id
         filteredVolunteers = myCurrentCallType.HasValue
            ? filteredVolunteers.OrderBy(v => v.CurrentCallType == myCurrentCallType.Value).ToList()
            : filteredVolunteers.OrderBy(v => v.Id).ToList();

        return filteredVolunteers;
    }
    public void UpdateVolunteer(int id, BO.Volunteer myVolunteer)
    {
        throw new NotImplementedException();
    }
}

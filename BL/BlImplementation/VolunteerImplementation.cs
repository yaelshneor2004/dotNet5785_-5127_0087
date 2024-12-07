using BlApi;
using BO;
using System.Linq;
namespace BlImplementation;



internal class VolunteerImplementation:IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public BO.MyRole Login(string username, string password)//CHECK
    {
        BO.Volunteer volunteer;//= BO.VolunteerInList.FirstOrDefault(v => v.fullName == username); throw new NotImplementedException();
        if (volunteer.Password == password)
        { return volunteer.Role; }
        else
            throw new UnauthorizedAccessException("Incorrect password.");

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
        // קבלת רשימת המתנדבים מה-DAL
        var volunteers = _dal.Volunteer.ReadAll();

        // סינון לפי ערך הסטטוס IsActive
        if (isActive.HasValue)
        {
            volunteers = volunteers.Where(v => v.IsActive == isActive.Value);
        }

        // מיפוי התוצאות ל-BO.VolunteerInList
        var filteredVolunteers = volunteers.Select(v => new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            IsActive = v.IsActive,
            TotalCallsHandled = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id && a.FinishType == DO.MyFinishType.Treated).Count(),
            TotalCallsCancelled = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id && (a.FinishType == DO.MyFinishType.SelfCancel || a.FinishType == DO.MyFinishType.ManagerCancel)).Count(),
            TotalCallsExpired = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id && a.FinishType == DO.MyFinishType.ExpiredCancel).Count(),
            CurrentCallId = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id && a.FinishCall == null).Select(a => (int?)a.CallId).FirstOrDefault(),
            CurrentCallType = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id && a.FinishCall == null).Select(a => _dal.Call.ReadAll(c => c.Id == a.CallId).Select(c => (BO.MyCurrentCallType?)c.CallType).FirstOrDefault()).FirstOrDefault() ?? BO.MyCurrentCallType.None
        }).ToList();

        // מיון לפי ערך השדה המוגדר או לפי Id
        if (myCurrentCallType.HasValue)
        {
            filteredVolunteers = filteredVolunteers.OrderBy(v => v.CurrentCallType == myCurrentCallType.Value).ToList();
        }
        else
        {
            filteredVolunteers = filteredVolunteers.OrderBy(v => v.Id).ToList();
        }

        return filteredVolunteers;
    }


    //public BO.VolunteerInList GetVolunteerList(bool? IsActive, BO.MyCurrentCallType? myCurrentCallType)//CHECK
    //{
    //    var filteredVolunteers = _dal.Volunteer;


    //    if (IsActive == null)
    //        return (BO.VolunteerInList)_dal.Volunteer;
    //    else
    //    {
    //        var a = _dal.Volunteer.Where(v => v.IsActive == true).ToList();
    //    }

    //    if (volunteerInList != null)
    //    {
    //        filteredVolunteers = SortVolunteers(filteredVolunteers, volunteerInList.Id);
    //    }
    //    else
    //        filteredVolunteers = filteredVolunteers.OrderBy(v => v.Id);

    //    return filteredVolunteers.Select(v => new VolunteerInList
    //    {
    //        Id = v.Id,
    //        FullName = v.FullName,
    //        Role = v.Role.ToString()
    //    }
    //    ).ToList();
    //}
    //public VolunteerInList GetVolunteerList(bool? IsActive, VolunteerInList? volunteerInList)
    //{
    //    var filteredVolunteers = volunteers.AsEnumerable();

    //    if (IsActive.HasValue)
    //    {
    //        filteredVolunteers = filteredVolunteers.Where(v => v.IsActive == IsActive.Value);
    //    }

    //    if (volunteerInList != null)
    //    {
    //        filteredVolunteers = SortVolunteers(filteredVolunteers, volunteerInList.Id); // לדוגמה: מיון לפי Id
    //    }
    //    else
    //    {
    //        filteredVolunteers = filteredVolunteers.OrderBy(v => v.Id);
    //    }

    //    return filteredVolunteers.Select(v => new VolunteerInList
    //    {
    //        Id = v.Id,
    //        FullName = v.FullName,
    //        Role = v.Role.ToString()
    //    }).ToList();
    //}
    public void UpdateVolunteer(int id, BO.Volunteer myVolunteer)
    {
        throw new NotImplementedException();
    }
}

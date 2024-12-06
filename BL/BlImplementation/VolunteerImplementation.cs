using BlApi;
using BO;

namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
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

    public BO.VolunteerInList GetVolunteerList(bool? IsActive, BO.VolunteerInList? volunteerInList)//CHECK
    {
        var filteredVolunteers = BO.VolunteerInList();

        if (IsActive.HasValue)
        {
           filteredVolunteers = filteredVolunteers.Where(v => v.IsActive == IsActive.Value);
        }
        if (volunteerInList != null)
        {
            filteredVolunteers = SortVolunteers(filteredVolunteers, volunteerInList.Id); 
        }
        else
            filteredVolunteers = filteredVolunteers.OrderBy(v => v.Id);

        return filteredVolunteers.Select(v => new VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            Role = v.Role.ToString()
        }
        ).ToList();
    }

    /*using System;

    public VolunteerInList GetVolunteerList(bool? IsActive, VolunteerInList? volunteerInList)
    {
        var filteredVolunteers = volunteers.AsEnumerable();

        if (IsActive.HasValue)
        {
            filteredVolunteers = filteredVolunteers.Where(v => v.IsActive == IsActive.Value);
        }

        if (volunteerInList != null)
        {
            filteredVolunteers = SortVolunteers(filteredVolunteers, volunteerInList.Id); // לדוגמה: מיון לפי Id
        }
        else
        {
            filteredVolunteers = filteredVolunteers.OrderBy(v => v.Id);
        }

        return filteredVolunteers.Select(v => new VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            Role = v.Role.ToString()
        }).ToList();
    }

    private IEnumerable<Volunteer> SortVolunteers(IEnumerable<Volunteer> volunteers, int sortBy)
    {
        // דוגמה למיון, לפי הצורך, שים לב לשדות האחרים
        return volunteers.OrderBy(v => v.Id);
    }

    // Other methods...

    public string Login(string username, string password)
    {
        var volunteer = volunteers.FirstOrDefault(v => v.Email == username && v.IsActive);
        if (volunteer != null)
        {
            if (volunteer.Password == password)
            {
                return volunteer.Role.ToString();
            }
            else
            {
                throw new UnauthorizedAccessException("Incorrect password.");
            }
        }
        else
        {
            throw new KeyNotFoundException("User does not exist or is not active.");
        }
    }

    public void InitializeDB()
    {
        throw new NotImplementedException();
    }

    public void ResetDB()
    {
        throw new NotImplementedException();
    }

    public int GetMaxRange()
    {
        throw new NotImplementedException();
    }

    public void SetMaxRange(int maxRange)
    {
        throw new NotImplementedException();
    }

    public DateTime GetClock()
    {
        throw new NotImplementedException();
    }

    public void ForwardClock(TimeUnit unit)
    {
        throw new NotImplementedException();
    }
}
*/

    public void UpdateVolunteer(int id, BO.Volunteer myVolunteer)
    {
        throw new NotImplementedException();
    }
}

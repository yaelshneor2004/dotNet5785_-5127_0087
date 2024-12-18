using BlApi;
using Helpers;
using System.Linq;
namespace BlImplementation;
internal class VolunteerImplementation:IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public BO.MyRole Login(string username, string password)
    {
        try
        {
            // Retrieve the list of volunteers from the DAL
            var volunteers = _dal.Volunteer.ReadAll();
            // Search for the user by username
            var user = volunteers.FirstOrDefault(v => v.FullName == username);
            password = VolunteerManager.Encrypt(password);
            if (user == null)
                throw new BO.BlUnauthorizedAccessException($"Username {username} does not exist");
            if (user.Password != password)
                throw new BO.BlUnauthorizedAccessException($"Password {password} is incorrect");
            return (BO.MyRole)user.Role;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"User with the username '{username}' does not exist in the system");
        }
    }
    public void AddVolunteer(BO.Volunteer myVolunteer)
    {
        try
        {
            // Validate input values

            VolunteerManager.ValidateVolunteerDetails(myVolunteer);
            // Attempt to add the new volunteer to DAL
            myVolunteer.Password=VolunteerManager.Encrypt(myVolunteer.Password);
            _dal.Volunteer.Create(VolunteerManager.ConvertFromBoToDo(myVolunteer));
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID {myVolunteer.Id} already exists.", ex);
        }
    }
    public void DeleteVolunteer(int volunteerId)
        {
            try
            {
                // Retrieve the volunteer details from DAL
                var volunteer = _dal.Volunteer.Read(volunteerId);
                // Check if the volunteer is currently handling any calls or has handled any calls in the past
                var currentAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.FinishCall == null).Any();
                var pastAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId).Any();
                if (currentAssignments || pastAssignments)
                    throw new BO.BlInvalidOperationException("Cannot delete volunteer who is currently handling or has handled calls.");
                // Attempt to delete the volunteer from DAL
                _dal.Volunteer.Delete(volunteerId);
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {volunteerId} does not exist.", ex);
            }
       }
    public BO.Volunteer GetVolunteerDetails(int id)
    {
        try
        {
           var volunteerData = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist."); ;
            return VolunteerManager.ConvertFromDoToBo(volunteerData);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist.", ex);
        }
    }
    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? isActive, BO.MySortInVolunteerInList? mySortInVolunteerInList)
    {
        var volunteers = _dal.Volunteer.ReadAll();

        // Filter by the IsActive status value
        if (isActive.HasValue)
            volunteers = volunteers.Where(v => v.IsActive == isActive.Value);
        var filteredVolunteers= volunteers.Select(v=>VolunteerManager.ConvertToVolunteerInList(v)).ToList();
        // Sort by the defined field value or by Id
        return VolunteerManager.SortVolunteers(filteredVolunteers, mySortInVolunteerInList.Value).ToList();
    }
    public void UpdateVolunteer(int id, BO.Volunteer myVolunteer)
    {
        try
        {
            // Retrieve requester details from DAL
            var volunteer = _dal.Volunteer.Read(id);
            // Verify that the requester is a manager or the same volunteer
            if (!volunteer.Role.Equals(BO.MyRole.Manager) && volunteer.Id != myVolunteer.Id)
                throw new BO.BlUnauthorizedAccessException("Only managers or the volunteer themselves can update the details.");
            VolunteerManager.ValidateVolunteerDetails(myVolunteer);
            var updatedVolunteer = VolunteerManager.ConvertFromBoToDo(myVolunteer);
            // Attempt to update the volunteer in DAL
            _dal.Volunteer.Update(updatedVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID {myVolunteer.Id} does not exist.", ex);
        }
    }

}

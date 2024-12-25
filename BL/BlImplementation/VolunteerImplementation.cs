using BlApi;
using BO;
using Helpers;
using System;
using System.Linq;
namespace BlImplementation;
internal class VolunteerImplementation:IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    /// Logs in a volunteer using their username and password.
    /// </summary>
    /// <param name="username">The username of the volunteer.</param>
    /// <param name="password">The password of the volunteer.</param>
    /// <returns>The role of the volunteer.</returns>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown when the username or password is incorrect.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the user does not exist in the system.</exception>
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
            throw new BO.BlDoesNotExistException($"User with the username '{username}' does not exist in the system",ex);
        }
    }
    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="myVolunteer">The volunteer details to add.</param>
    /// <exception cref="BO.BlAlreadyExistsException">Thrown when a volunteer with the same ID already exists.</exception>
    public void AddVolunteer(BO.Volunteer myVolunteer)
    {
        try
        {
            VolunteerManager.ValidateVolunteerDetails(myVolunteer);
            // Attempt to add the new volunteer to DAL
            myVolunteer.Password = myVolunteer.Password != null ? VolunteerManager.Encrypt(myVolunteer.Password) : null;
            _dal.Volunteer.Create(VolunteerManager.ConvertFromBoToDo(myVolunteer));
            VolunteerManager.Observers.NotifyListUpdated();                                                  

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID {myVolunteer.Id} already exists.", ex);
        }
    }
    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to delete.</param>
    /// <exception cref="BO.BlInvalidOperationException">Thrown when the volunteer is currently handling or has handled calls.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the volunteer does not exist.</exception>
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
            VolunteerManager.Observers.NotifyListUpdated();  	

        }
        catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {volunteerId} does not exist.", ex);
            }
       }
    /// <summary>
    /// Retrieves the details of a specific volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer to retrieve.</param>
    /// <returns>The details of the volunteer.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the volunteer does not exist.</exception>
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
    /// <summary>
    /// Retrieves the details of a specific volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer to retrieve.</param>
    /// <returns>The details of the volunteer.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the volunteer does not exist.</exception>
    public IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? isActive, BO.MySortInVolunteerInList? mySortInVolunteerInList)
    {
        var volunteers = _dal.Volunteer.ReadAll();

        // Filter by the IsActive status value
        if (isActive.HasValue)
            volunteers = volunteers.Where(v => v.IsActive == isActive.Value);
        var filteredVolunteers= volunteers.Select(v=>VolunteerManager.ConvertToVolunteerInList(v)).ToList();
        // Sort by the defined field value or by Id
        return VolunteerManager.SortVolunteers(filteredVolunteers, mySortInVolunteerInList ?? new BO.MySortInVolunteerInList()).ToList();
    }


    public IEnumerable<VolunteerInList> GetFilterVolunteerList(BO.MyCallType filter)
    {
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();
        var volunteerList = volunteers.Select(v => VolunteerManager.ConvertToVolunteerInList(v)).ToList();
        return filter != BO.MyCallType.None ? volunteerList.Where(v => v.CurrentCallType == filter).ToList() : volunteerList;
    }

    /// <summary>
    /// Updates the details of a specific volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer to update.</param>
    /// <param name="myVolunteer">The updated volunteer details.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown when the requester is not authorized to update the details.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the volunteer does not exist.</exception>
    public void UpdateVolunteer(int id, BO.Volunteer myVolunteer)
    {
        try
        {
            // Retrieve requester details from DAL
            var volunteer = _dal.Volunteer.Read(id);
            // Verify that the requester is a manager or the same volunteer
            if (volunteer?.Role != null && !volunteer.Role.Equals(BO.MyRole.Manager) && volunteer.Id != myVolunteer?.Id)
                throw new BO.BlUnauthorizedAccessException("Only managers or the volunteer themselves can update the details.");
            VolunteerManager.ValidateVolunteerDetails(myVolunteer);
            var updatedVolunteer = VolunteerManager.ConvertFromBoToDo(myVolunteer);
            // Attempt to update the volunteer in DAL
            _dal.Volunteer.Update(updatedVolunteer);
            VolunteerManager.Observers.NotifyItemUpdated(updatedVolunteer.Id);  
            VolunteerManager.Observers.NotifyListUpdated();  

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID {myVolunteer.Id} does not exist.", ex);
        }
    }

    public void AddObserver(Action listObserver) =>
VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5

}


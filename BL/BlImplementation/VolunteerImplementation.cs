using BlApi;
using BO;
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

            // Check if the user exists
            if (user == null)
            {
                throw new BO.BlUnauthorizedAccessException($"Username {username} does not exist");
            }

            // Check if the password is correct
            if (user.Password != password)
            {
                throw new BO.BlUnauthorizedAccessException($"Password {password} is incorrect");
            }

            // Return the user's role
            return (BO.MyRole)user.Role;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"User with the username '{username}' does not exist in the system",ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlException("An error occurred while retrieving volunteer details.", ex);
        }
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
        try
        {
            // Retrieve the volunteer details from DAL
            var volunteerData = _dal.Volunteer.Read(id);

            // Create the BO.Volunteer object
            var volunteer = new BO.Volunteer
            {
                Id = volunteerData.Id,
                FullName = volunteerData.FullName,
                Phone = volunteerData.Phone,
                Email = volunteerData.Email,
                Password = volunteerData.Password,
                Address = volunteerData.Address,
                Latitude = volunteerData.Latitude,
                Longitude = volunteerData.Longitude,
                Role = (BO.MyRole)volunteerData.Role,
                IsActive = volunteerData.IsActive,
                MaxDistance = volunteerData.MaxDistance,
                TypeDistance = (BO.MyTypeDistance)volunteerData.TypeDistance,
                TotalCallsHandled = _dal.Assignment.ReadAll(a=>a.VolunteerId==id&& a.FinishType == DO.MyFinishType.Treated).Count(),
                TotalCallsCancelled = _dal.Assignment.ReadAll(a => a.VolunteerId ==id && (a.FinishType == DO.MyFinishType.SelfCancel || a.FinishType == DO.MyFinishType.ManagerCancel)).Count(),
                TotalCallsExpired = _dal.Assignment.ReadAll(a => a.VolunteerId == id && a.FinishType == DO.MyFinishType.ExpiredCancel).Count(),
            };

            // Check if there is a call in progress for the volunteer
            var assignment = _dal.Assignment.ReadAll(a => a.VolunteerId == id && a.FinishCall == null).FirstOrDefault();

            if (assignment != null)
            {
                // Retrieve the call details
                var callData = _dal.Call.Read(assignment.CallId);

                // Create the BO.CallInProgress object
                var callInProgress = new BO.CallInProgress
                {
                    Id = assignment.Id,
                    CallId = assignment.CallId,
                    CallType = (BO.MyCallType)callData.CallType,
                    Description = callData.Description,
                    Address = callData.Address,
                    StartTime = callData.OpenTime,
                    MaxEndTime = callData.MaxFinishCall,
                    StartTreatmentTime = assignment.StartCall,
                    DistanceFromVolunteer =VolunteerManager.CalculateDistance(volunteer.Latitude, volunteer.Longitude, callData.Latitude, callData.Longitude),
                    Status = VolunteerManager.DetermineCallStatus(callData.MaxFinishCall)
                };

                // Add the call in progress to the volunteer object
                volunteer.CurrentCall = callInProgress;
            }
            else
                volunteer.CurrentCall = null;

            return volunteer;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlException("An error occurred while retrieving volunteer details.", ex);
        }
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
        try
        {
            // Retrieve requester details from DAL
            var volunteer = _dal.Volunteer.Read(id);

            // Verify that the requester is a manager or the same volunteer
            if (!volunteer.Role.Equals(BO.MyRole.Manager) && volunteer.Id != myVolunteer.Id)
            {
                throw new BO.BlUnauthorizedAccessException("Only managers or the volunteer themselves can update the details.");
            }

            // Validate input values
            ValidateVolunteerDetails(myVolunteer);
            var updatedVolunteer = new DO.Volunteer
            {
                // Update the fields that are allowed to be updated
                Id = myVolunteer.Id,
                FullName = myVolunteer.FullName,
                Phone = myVolunteer.Phone,
                Email = myVolunteer.Email,
                Password = myVolunteer.Password,
                Address = myVolunteer.Address,
                Latitude = myVolunteer.Latitude,
                Longitude = myVolunteer.Longitude,
                IsActive = myVolunteer.IsActive,
                MaxDistance = myVolunteer.MaxDistance,
                TypeDistance = (DO.MyTypeDistance)myVolunteer.TypeDistance,
                Role = volunteer.Role.Equals(BO.MyRole.Manager) ? (DO.MyRole)myVolunteer.Role :_dal.Volunteer.Read(myVolunteer.Id).Role // Only managers can update the role
            };
            // Attempt to update the volunteer in DAL
            _dal.Volunteer.Update(updatedVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID {myVolunteer.Id} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlException("An error occurred while updating volunteer details.", ex);
        }
    }

    // Helper method to validate volunteer details
    private void ValidateVolunteerDetails(BO.Volunteer volunteer)
    {
        // Validate email format
        if (!IsValidEmail(volunteer.Email))
        {
            throw new BO.BlValidationException("Invalid email format.");
        }

        // Validate numeric fields
        if (!IsValidPhone(volunteer.Phone))
        {
            throw new BO.BlValidationException("Invalid phone number.");
        }

        // Additional validation for other fields can be added here

        // Validate address and calculate latitude and longitude
        var coordinates = GeocodeAddress(volunteer.Address);
        volunteer.Latitude = coordinates.Latitude;
        volunteer.Longitude = coordinates.Longitude;
    }

    // Helper method to validate email format
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Helper method to validate phone number format (basic example)
    private bool IsValidPhone(string phone)
    {
        return phone.All(char.IsDigit) && phone.Length == 10;
    }

    // Helper method to geocode address (dummy implementation)
    private (double Latitude, double Longitude) GeocodeAddress(string address)
    {
        // Implement actual geocoding logic here
        return (0.0, 0.0);
    }

}

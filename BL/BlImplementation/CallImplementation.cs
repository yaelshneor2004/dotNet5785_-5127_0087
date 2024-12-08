using BlApi;

using Helpers;

namespace BlImplementation;

internal class CallImplementation:ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddCall(BO.Call myCall)
    {
        throw new NotImplementedException();
    }
    public int[] CallAmount()
    {
        throw new NotImplementedException();
    }

    public void DeleteCall(int callId)
    {
        try
        {
            // Retrieve call details from the data layer
            var callData = _dal.Call.Read(callId);
            // Check if the call is open and has never been assigned
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId).ToList();
            if (assignments. != BO.MyCallStatus.Open || assignments.Any())
            {
                throw new BO.BlException("Only open calls that have never been assigned can be deleted.");
            }
            // Attempt to delete the call in the data layer
            _dal.Call.Delete(callId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlException("An error occurred while deleting the call.", ex);
        }
    }
    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            // Retrieve call details from the data layer
            var callData = _dal.Call.Read(callId);
            // Retrieve the list of assignments for the call from the data layer
            var assignmentsData = _dal.Assignment.ReadAll(a => a.CallId == callId).ToList();
            // Map call details to BO.Call object
            var callDetails = new BO.Call
            {
                Id = callData.Id,
                Type =(BO.MyCallType) callData.CallType,
                Description = callData.Description,
                Address = callData.Address,
                Latitude = callData.Latitude,
                Longitude = callData.Longitude,
                StartTime = callData.OpenTime,
                MaxEndTime = callData.MaxFinishCall,
                Status =CallManager.CalculateCallStatus(assignmentsData, callData.MaxFinishCall),
                Assignments = assignmentsData.Select(a => new BO.CallAssignInList
                {
                    VolunteerId = a.VolunteerId != 0 ? a.VolunteerId : (int?)null,
                    VolunteerName = a.VolunteerId != 0 ? _dal.Volunteer.Read(a.VolunteerId)?.FullName : null,
                    EndTreatmentTime = a.FinishCall,
                    EndType = a.FinishType != null ? (BO.MyFinishType)a.FinishType : (BO.MyFinishType?)null
                }).ToList()

            };
            return callDetails;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlException("An error occurred while retrieving call details.", ex);
        }
    }

    public BO.CallInList GetCallList(BO.CallInList? CallFilterBy, object? obj, CallInList? CallSortBy)
    {
        throw new NotImplementedException();
    }

    public void SelectCallToTreat(int idV, int idC)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ClosedCallInList> SortClosedCalls(int idV, MyCallType? callType)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenCallInList> SortOpenedCalls(int idV, MyCallType? callType, OpenCallInList openCallInList)
    {
        throw new NotImplementedException();
    }

    public void UpdateCallDetails(BO.Call call)
    {
        try
        {
            // Validate the format of the values
            ValidateCallFormat(call);

            // Validate the logical correctness of the values
            ValidateCallLogic(call);

            // Create a data object of type DO.Call
            var doCall = new DO.Call
            {
                Id = call.Id,
                CallType =(DO.MyCallType) call.Type,
                Address = call.Address?? "",
                Latitude = call.Latitude ?? 0,
                Longitude = call.Longitude ?? 0,
                OpenTime = call.StartTime,
                Description = call.Description,
                MaxFinishCall = call.MaxEndTime
            };

            // Attempt to update the call in the data layer
            _dal.Call.Update(doCall);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {call.Id} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlException("An error occurred while updating call details.", ex);
        }
    }

    // Method to validate the format of the values
    private void ValidateCallFormat(BO.Call call)
    {
        if (string.IsNullOrWhiteSpace(call.Address))
        {
            throw new BO.BlException("Address cannot be empty.");
        }

        if (call.Latitude.HasValue && (call.Latitude < -90 || call.Latitude > 90))
        {
            throw new BO.BlException("Latitude must be between -90 and 90.");
        }

        if (call.Longitude.HasValue && (call.Longitude < -180 || call.Longitude > 180))
        {
            throw new BO.BlException("Longitude must be between -180 and 180.");
        }
    }

    // Method to validate the logical correctness of the values
    private void ValidateCallLogic(BO.Call call)
    {
        if (call.MaxEndTime.HasValue && call.MaxEndTime <= call.StartTime)
        {
            throw new BO.BlException("Max end time must be later than start time.");
        }

        // Validate the address as a real-world address
        var geoCoordinates = GetCoordinatesFromAddress(call.Address);
        if (geoCoordinates == null)
        {
            throw new BO.BlException("Address is not valid.");
        }

        // Update the latitude and longitude based on the address
        call.Latitude = geoCoordinates.Latitude;
        call.Longitude = geoCoordinates.Longitude;
    }

    // Helper method to calculate latitude and longitude based on address
    private (double Latitude, double Longitude)? GetCoordinatesFromAddress(string address)
    {
        // Here you would use an external service to calculate the coordinates based on the address
        // For example purposes, returning a fixed value
        return (32.0853, 34.7818); // Tel Aviv
    }


    public void UpdateCancelTreatment(int idV, int idC)
    {
        throw new NotImplementedException();
    }

    public void UpdateEndTreatment(int idV, int idC)
    {
        throw new NotImplementedException();
    }
}

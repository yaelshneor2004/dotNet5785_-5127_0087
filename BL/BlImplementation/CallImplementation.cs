using BlApi;

using Helpers;
using Newtonsoft.Json;
using static Helpers.CallManager;

namespace BlImplementation;

internal class CallImplementation:ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddCall(BO.Call call)
    {
        try
        {
            // Validate the format of the values
            CallManager. ValidateCallFormat(call);

            // Validate the logical correctness of the values
           CallManager. ValidateCallLogic(call);

            // Create a new data object of type DO.Call
            var newCall = new DO.Call
            {
                Id =call.Id,
                CallType = (DO.MyCallType)call.Type,
                Address = call.Address??"",
                Latitude = call.Latitude ?? 0,
                Longitude = call.Longitude ?? 0,
                OpenTime = DateTime.Now,
                Description = call.Description,
                MaxFinishCall = call.MaxEndTime
            };

            // Attempt to add the new call in the data layer
            _dal.Call.Create(newCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Call with ID {call.Id} already exists.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlException("An error occurred while adding the call.", ex);
        }
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
                Type = (BO.MyCallType)callData.CallType,
                Description = callData.Description,
                Address = callData.Address,
                Latitude = callData.Latitude,
                Longitude = callData.Longitude,
                StartTime = callData.OpenTime,
                MaxEndTime = callData.MaxFinishCall,
                Status = CallManager.GetCallStatus(callData, assignmentsData),
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

    public void UpdateCall(BO.Call myCall)
    {
        try
        {
            // Validate the format of the values
            CallManager.ValidateCallFormat(myCall);

            // Validate the logical correctness of the values
            CallManager.ValidateCallLogic(myCall);

            // Create a data object of type DO.Call
            var doCall = new DO.Call
            {
                Id = myCall.Id,
                CallType = (DO.MyCallType)myCall.Type,
                Address = myCall.Address ?? "",
                Latitude = myCall.Latitude ?? 0,
                Longitude = myCall.Longitude ?? 0,
                OpenTime = myCall.StartTime,
                Description = myCall.Description,
                MaxFinishCall = myCall.MaxEndTime
            };

            // Attempt to update the call in the data layer
            _dal.Call.Update(doCall);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {myCall.Id} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlException("An error occurred while updating call details.", ex);
        }
    }
    public void UpdateCancelTreatment(int idV, int idC)
    {
        throw new NotImplementedException();
    }
    public void CompleteAssignment(int volunteerId, int assignmentId)
    {
        try
        {
            // Retrieve assignment details from the data layer
            var assignment = _dal.Assignment.Read(assignmentId);

            // Check if the volunteer is authorized to complete the assignment
            if (assignment.VolunteerId != volunteerId)
            {
                throw new BO.BlException("Unauthorized: The volunteer does not match the assignment.");
            }

            // Check if the assignment is still open
            if (assignment.FinishCall.HasValue)
            {
                throw new BO.BlException("The assignment is already completed or canceled.");
            }

            // Check if the call is not expired
            var call = _dal.Call.Read(assignment.CallId);
            if (call.MaxFinishCall.HasValue && ClockManager.Now > call.MaxFinishCall.Value)
            {
                throw new BO.BlException("The call has expired.");
            }

            // Update assignment details
            assignment.FinishType =DO.MyFinishType.Treated;
            assignment.FinishCall = ClockManager.Now;

            // Update the assignment in the data layer
            _dal.Assignment.Update(assignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlException("An error occurred while completing the assignment.", ex);
        }
    }

}

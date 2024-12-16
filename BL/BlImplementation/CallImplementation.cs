using BlApi;
using BO;
using Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Helpers.CallManager;
namespace BlImplementation;
internal class CallImplementation:ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddCall(BO.Call call)
    {
            // Validate the format of the values
            CallManager. ValidateCallFormat(call);
            // Validate the logical correctness of the values
           CallManager. ValidateCallLogic(call);


        // Attempt to add the new call in the data layer
        _dal.Call.Create(ConvertBOToDO(call)); 
    }

    public int[] CallAmount()
    {
        var calls = from call in _dal.Call.ReadAll()
                    let boCall = ConvertFromDoToBo(call)
                    group boCall by boCall.Status into groupedCalls
                    select new
                    {
                        Status = groupedCalls.Key,
                        Count = groupedCalls.Count()
                    };

        var callAmounts = new int[Enum.GetValues(typeof(MyCallStatus)).Length];
        foreach (var group in calls)
            callAmounts[(int)group.Status] = group.Count;
        return callAmounts;
    }

    public void DeleteCall(int callId)
    {
        try
        {
            // Retrieve call details from the data layer
            var callData = _dal.Call.Read(callId);
            // Check if the call is open and has never been assigned
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId).ToList();
            var callBo = CallManager.ConvertFromDoToBo(callData);
            if (callBo.Status!= BO.MyCallStatus.Open || assignments.Any())
                throw new BO.BlUnauthorizedAccessException("Only open calls that have never been assigned can be deleted.");
            // Attempt to delete the call in the data layer
            _dal.Call.Delete(callId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlErrorException("An error occurred while deleting the call.", ex);
        }
    }
    public BO.Call GetCallDetails(int callId)
    { 
        try
        {
            // Retrieve call details from the data layer
            var callData = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist.");
            // Convert DO.Call to BO.Call and return
            return CallManager.ConvertFromDoToBo(callData);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlErrorException("An error occurred while retrieving call details.", ex);
        }
    }

    public IEnumerable< BO.CallInList> GetCallList(BO.MySortInCallInList? callFilter, object? filterValue, BO.MySortInCallInList? callSort)
    {
        var calls = _dal.Call.ReadAll();
        var callsBo = calls.Select(callData =>CallManager.ConvertToCallInList(callData)).Distinct().ToList();

        if (callFilter.HasValue && filterValue != null)
            callsBo = callsBo.Where(call =>CallManager.GetFieldValue(call, callFilter.Value)==(filterValue)).ToList();
        return CallManager.SortCalls(callsBo, callSort.Value).ToList();
    }

    public void SelectCallToTreat(int idV, int idC)
    {
        try
        {
            // Retrieve call details from the data layer
            var call = _dal.Call.Read(idC);

            // Retrieve all assignments for the call
            var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == idC).ToList();

            // Check if there are any open assignments for the call
            if (assignments.Any() && CallManager.IsOpenAssignment(assignments))
                throw new BO.BlInvalidOperationException("The call is already being treated by another volunteer.");

            // Check if the call has expired
            if (ClockManager.Now > call.MaxFinishCall)
                throw new BO.BlInvalidOperationException("The call has expired.");

            _dal.Assignment.Create(new DO.Assignment(0, idC, idV, ClockManager.Now, null, null));
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call or assignment with ID {idC} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlErrorException("An error occurred while selecting the call to treat.", ex);
        }
    }

    public IEnumerable<BO.ClosedCallInList> SortClosedCalls(int idV, MyCallType? callType, CloseCall? closeCall)
    {
        var closeList = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == idV && CallManager.CloseCondition(a)).Select(a => convertAssignmentToClosed(a)).ToList();
        closeList = callType.HasValue ? closeList.Where(call => call.Type == callType.Value).ToList() : closeList;
        return CallManager.SortClosedCallsByField(closeList, closeCall);
    }

    public IEnumerable<BO.OpenCallInList> SortOpenedCalls(int idV, MyCallType? callType,BO.OpenedCall? openedCall)
    {
        var openCalls = _dal.Assignment.ReadAll().Where(a=>a.VolunteerId==idV/*&&CallManager.OpenCondition(a)*/).Select(a => convertAssignmentToOpened(a)).ToList();
        openCalls = callType.HasValue ? openCalls.Where(call => call.Type == callType.Value).ToList() : openCalls;
        return CallManager.SortOpenCallsByField(openCalls, openedCall);
    }

    public void UpdateCall(BO.Call myCall)
    {
        try
        {
            // Validate the format of the values
            CallManager.ValidateCallFormat(myCall);
            // Validate the logical correctness of the values
            CallManager.ValidateCallLogic(myCall);
            // Attempt to update the call in the data layer
            _dal.Call.Update(CallManager.ConvertBOToDO(myCall));
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {myCall.Id} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlErrorException("An error occurred while updating call details.", ex);
        }
    }
    public void UpdateCancelTreatment(int idV, int idC)
    {
        try
        {
            // Retrieve assignment details from the data layer
            var assignment = _dal.Assignment.Read(idC);

            // Check authorization: the requester must be a manager or the volunteer assigned to the task
            if (assignment.VolunteerId != idV || !CallManager.IsManager(idV))
                throw new BO.BlUnauthorizedAccessException("The requester is not authorized to cancel this assignment.");

            // Check that the assignment is still open and not already completed or canceled
            if (assignment.FinishCall.HasValue || assignment.FinishType.HasValue)
                throw new BO.BlInvalidOperationException("The assignment has already been completed or canceled.");
            // Create a new assignment object with the updated finish time and finish type
            var updatedAssignment = assignment with
            {
                FinishCall = ClockManager.Now,
                FinishType = assignment.VolunteerId == idV ? DO.MyFinishType.SelfCancel : DO.MyFinishType.ManagerCancel
            };
            // Attempt to update the assignment entity in the data layer
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the assignment does not exist, throw an appropriate exception to the presentation layer
            throw new BO.BlDoesNotExistException($"Assignment with ID {idC} does not exist.", ex);
        }
        catch (Exception ex)
        {
            // If another error occurred while updating the assignment, throw a generic exception
            throw new BO.BlErrorException("An error occurred while canceling the assignment.", ex);
        }
    }
    public void UpdateCompleteAssignment(int volunteerId, int assignmentId)
    {
        try
        {
            // Retrieve assignment details from the data layer
            var assignment = _dal.Assignment.Read(assignmentId);

            // Check authorization: the volunteer must be the one assigned to the task
            if (assignment.VolunteerId != volunteerId)
                throw new BO.BlUnauthorizedAccessException("The volunteer is not authorized to complete this assignment.");

            // Check that the assignment is still open and not already completed or canceled
            if (assignment.FinishCall.HasValue || assignment.FinishType.HasValue)
                throw new BO.BlInvalidOperationException("The assignment has already been closed or canceled.");

            // Create a new assignment object with the updated finish time and finish type
            var updatedAssignment = assignment with
            {
                FinishCall = ClockManager.Now,
                FinishType = DO.MyFinishType.Treated
            };

            // Attempt to update the assignment entity in the data layer
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the assignment does not exist, throw an appropriate exception to the presentation layer
            throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            // If another error occurred while updating the assignment, throw a generic exception
            throw new BO.BlErrorException("An error occurred while completing the assignment.", ex);
        }
    }
}

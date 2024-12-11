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
        var calls = _dal.Call.ReadAll();
        var callsBo = calls.Select(callData => ConvertFromDoToBo(callData)).ToList(); // המרת הקריאות ל-BO
        var callAmounts = new int[Enum.GetValues(typeof(MyCallStatus)).Length];
        var groupedCalls = callsBo.GroupBy(c => c.Status)
                                .Select(group => new
                                {
                                    Status = group.Key,
                                    Count = group.Count()
                                });
        foreach (var group in groupedCalls)
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
            callsBo = callsBo.Where(call =>CallManager.GetFieldValue(call, callFilter.Value)?.Equals(filterValue) == true).ToList();
        return CallManager.SortCalls(callsBo, callSort.Value).ToList();
    }
    public void SelectCallToTreat(int idV, int idC)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.ClosedCallInList> SortClosedCalls(int idV, MyCallType? callType, CloseCallInList? closeCallInList)
    {
        var assignmennts = _dal.Assignment.ReadAll();
        assignmennts=assignmennts.Where(a=>a.VolunteerId == idV&&a.FinishType==null);

    }

    public IEnumerable<BO.OpenCallInList> SortOpenedCalls(int idV, MyCallType? callType, OpenCallInList openCallInList)
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

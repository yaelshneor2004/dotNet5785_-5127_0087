using BlApi;
using BO;
using DO;
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
    
    /// <summary>
    /// Calculates the number of calls for each call status.
    /// </summary>
    /// <returns>An array of integers representing the count of calls for each status.</returns>
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
    /// <summary>
    /// Deletes a call if it is open and has never been assigned.
    /// </summary>
    /// <param name="callId">The ID of the call to delete.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the call is not open or has been assigned.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call does not exist.</exception>
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
    }
    /// <summary>
    /// Retrieves the details of a call by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call to retrieve.</param>
    /// <returns>A BO.Call object with the call details.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call does not exist.</exception>
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
    }
        public IEnumerable<BO.CallInList> GetCallList(BO.MySortInCallInList? callFilter, object? filterValue, BO.MySortInCallInList? callSort)
        {
            var calls = _dal.Call.ReadAll();
            var callsBo = calls.Select(callData => CallManager.ConvertToCallInList(callData)).Distinct().ToList();
    /// <summary>
    /// Retrieves a list of calls, optionally filtered and sorted based on specified criteria.
    /// </summary>
    /// <param name="callFilter">The field to filter calls by.</param>
    /// <param name="filterValue">The value to filter calls by.</param>
    /// <param name="callSort">The field to sort calls by.</param>
    /// <returns>A filtered and sorted list of BO.CallInList objects.</returns>
    public IEnumerable< BO.CallInList> GetCallList(BO.MySortInCallInList? callFilter, object? filterValue, BO.MySortInCallInList? callSort)
    {
        var calls = _dal.Call.ReadAll();
        var callsBo = calls.Select(callData =>CallManager.ConvertToCallInList(callData)).Distinct().ToList();

            if (callFilter.HasValue && filterValue != null)
            {
                switch (callFilter)
                {
                    case BO.MySortInCallInList.Type:
                        if (Enum.TryParse(typeof(BO.MyCallType), filterValue.ToString(), out var resultO))
                            callsBo = callsBo.Where(c => c.Type == (BO.MyCallType)resultO).ToList();
                        break;

                    case BO.MySortInCallInList.StartTime:
                        if (DateTime.TryParse(filterValue.ToString(), out var resultT))
                            callsBo = callsBo.Where(c => c.StartTime ==(DateTime) resultT).ToList();
                        break;

                    case BO.MySortInCallInList.TimeRemaining:
                        if (TimeSpan.TryParse(filterValue.ToString(), out var resultTR))
                            callsBo = callsBo.Where(c => c.TimeRemaining ==(TimeSpan) resultTR).ToList();
                        break;

                    case BO.MySortInCallInList.CompletionTime:
                        if (TimeSpan.TryParse(filterValue.ToString(), out var resultCT))
                            callsBo = callsBo.Where(c => c.CompletionTime ==(TimeSpan) resultCT).ToList();
                        break;

                    case BO.MySortInCallInList.Status:
                        if (Enum.TryParse(typeof(BO.MyCallStatus), filterValue.ToString(), out var resultS))
                            callsBo = callsBo.Where(c => c.Status == (BO.MyCallStatus)resultS).ToList();
                        break;

                    case BO.MySortInCallInList.TotalAssignments:
                        if (int.TryParse(filterValue.ToString(), out var resultTA))
                            callsBo = callsBo.Where(c => c.TotalAssignments == resultTA).ToList();
                        break;

                    default:
                        break;
                }
            }

            return CallManager.SortCalls(callsBo, callSort.Value).ToList();
        }


        public void SelectCallToTreat(int idV, int idC)
        if (callFilter.HasValue && filterValue != null)
            callsBo = callsBo.Where(call =>CallManager.GetFieldValue(call, callFilter.Value)==(filterValue)).ToList();
        return CallManager.SortCalls(callsBo, callSort.Value).ToList();
    }
    /// <summary>
    /// Selects a call to treat by creating an assignment for the volunteer.
    /// </summary>
    /// <param name="idV">The ID of the volunteer.</param>
    /// <param name="idC">The ID of the call.</param>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the call is already being treated or has expired.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call or assignment does not exist.</exception>
    public void SelectCallToTreat(int idV, int idC)
    {
        try
        {
            // Retrieve call details from the data layer
            var call = _dal.Call.Read(idC);

            // Retrieve all assignments for the call


            var assignments = _dal.Assignment.ReadAll();
            assignments = assignments.Where(a => a.CallId == idC).ToList();

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
    }
    /// <summary>
    /// Sorts closed calls for a specific volunteer, optionally filtered by call type and sorted by a specified field.
    /// </summary>
    /// <param name="idV">The ID of the volunteer.</param>
    /// <param name="callType">The type of calls to filter by</param>
    /// <param name="closeCall">The field to sort the closed calls by</param>
    /// <returns>A sorted and optionally filtered collection of BO.ClosedCallInList objects.</returns>
    public IEnumerable<BO.ClosedCallInList> SortClosedCalls(int idV,BO. MyCallType? callType, CloseCall? closeCall)
    {
        var closeList = _dal.Assignment.ReadAll();
       var closeListt= closeList.Where(a => a.VolunteerId == idV && CallManager.CloseCondition(a)).Select(a => convertAssignmentToClosed(a)).ToList();
        closeListt = callType.HasValue ? closeListt.Where(call => call.Type == callType.Value).ToList() : closeListt;
        return CallManager.SortClosedCallsByField(closeListt, closeCall);
    }

    /// <summary>
    /// Sorts opened calls for a specific volunteer, optionally filtered by call type and sorted by a specified field.
    /// </summary>
    /// <param name="idV">The ID of the volunteer.</param>
    /// <param name="callType">The type of calls to filter by</param>
    /// <param name="openedCall">The field to sort the opened calls by</param>
    /// <returns>A sorted and optionally filtered collection of BO.OpenCallInList objects.</returns>
    public IEnumerable<BO.OpenCallInList> SortOpenedCalls(int idV,BO. MyCallType? callType,BO.OpenedCall? openedCall)
    {
        var openCalls = _dal.Assignment.ReadAll().Where(a=>a.VolunteerId==idV&&CallManager.OpenCondition(a)).Select(a => convertAssignmentToOpened(a)).ToList();
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
    }
    public void UpdateCancelTreatment(int idV, int idC)
    {
        try
        {
            // Retrieve assignment details from the data layer
            var assignment = _dal.Assignment.Read(a => a.CallId == idC);
            

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
            if (CallManager.IsManager(idV))
            {
                var volunteer = _dal.Volunteer.Read(assignment.VolunteerId);
                var subject = "Assignment Cancelled";
                var body = $"Your assignment for call {assignment.CallId} has been cancelled.";
                SendEmail(volunteer.Email, subject, body);
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the assignment does not exist, throw an appropriate exception to the presentation layer
            throw new BO.BlDoesNotExistException($"Assignment with ID {idC} does not exist.", ex);
        }
    }
    /// <summary>
    /// Updates an assignment as complete for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the volunteer is not authorized to complete the assignment.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the assignment has already been completed or canceled.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the assignment does not exist.</exception>
    public void UpdateCompleteAssignment(int volunteerId, int assignmentId)
    {
        try
        {
            var assignment = _dal.Assignment.Read(a => a.Id == assignmentId);

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
    }
}

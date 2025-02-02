using BlApi;
using BO;
using DalApi;
using Helpers;
using System.Collections.Generic;
using static Helpers.CallManager;

namespace BlImplementation;

/// <summary>
/// Implementation of call-related operations.
/// </summary>
internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Adds a new call to the system.
    /// </summary>
    /// <param name="call">The call details to add.</param>
    public void AddCall(BO.Call call)
    {
        try
        {

            AdminManager.ThrowOnSimulatorIsRunning();
            // Validate the format of the values
            CallManager.ValidateCallFormat(call);
            // Validate the logical correctness of the values
            CallManager.ValidateCallLogic(call);
            // Attempt to add the new call in the data layer
            DO.Call callDo = ConvertBOToDO(call);
            DO.Call newCall;
            lock (AdminManager.BlMutex)
            {
                _dal.Call.Create(callDo);
                newCall = _dal.Call.ReadAll().Last();
                _ = CallManager.AddCallCoordinatesAsync(newCall);
              _=CallManager.AddCallSendEmailAsync(newCall);
            }
            CallManager.Observers.NotifyListUpdated();

        }
        catch (BO.BlNullPropertyException ex)
        {
            throw new BO.BlNullPropertyException(ex.Message);
        }
        catch (BO.BlInvalidOperationException ex)
        {
            throw new BO.BlInvalidOperationException(ex.Message);
        }
        catch (BO.BlTemporaryNotAvailableException ex)
        {
            throw new BO.BlTemporaryNotAvailableException(ex.Message);
        }

    }

    /// <summary>
    /// Retrieves the amount of calls grouped by their status.
    /// </summary>
    /// <returns>An array of call counts by status.</returns>
   public class CallStatusInfo
    {
        public int Status { get; set; }
        public int Count { get; set; }
    }
    public int[] CallAmount()
    {
        IEnumerable<CallStatusInfo> callStatuses;

        lock (AdminManager.BlMutex)
        {
            callStatuses = from call in _dal.Call.ReadAll()
                           group call by (int)CallManager.GetCallStatus(call) into g
                           orderby g.Key
                           select new CallStatusInfo { Status = g.Key, Count = g.Count() };
        }
            // Prepare an array with six cells (or more if there are more statuses)
            var callAmounts = new int[Enum.GetValues(typeof(BO.MyCallStatus)).Length - 1];

            // Fill the array with values from the grouping, or zeros if there are no calls in the status
            foreach (var status in callStatuses)
            {
                callAmounts[status.Status] = status.Count;
            }

            return callAmounts;
    }


    /// <summary>
    /// Deletes a call from the system.
    /// </summary>
    /// <param name="callId">The ID of the call to delete.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown when the call cannot be deleted due to its status or assignments.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the call does not exist.</exception>
    public void DeleteCall(int callId)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            // Retrieve call details from the data layer
            DO.Call? callData;
           int assignments;
            lock (AdminManager.BlMutex)
            {
                callData = _dal.Call.Read(callId); 
                assignments = _dal.Assignment.ReadAll(a => a.CallId == callId).Count();
            }
            var callBo = CallManager.ConvertFromDoToBo(callData ?? throw new BO.BlNullPropertyException(nameof(callData)));
            if (callBo.Status != BO.MyCallStatus.Open && callBo.Status != BO.MyCallStatus.OpenAtRisk &&assignments!=0)
                throw new BO.BlUnauthorizedAccessException("Only open calls that have never been assigned can be deleted.");
            // Attempt to delete the call in the data layer
            lock (AdminManager.BlMutex)
                _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist.", ex);
        }
        catch (BO.BlTemporaryNotAvailableException ex)
        {
            throw new BO.BlTemporaryNotAvailableException(ex.Message);
        }
    }



    /// <summary>
    /// Retrieves the details of a specific call.
    /// </summary>
    /// <param name="callId">The ID of the call to retrieve.</param>
    /// <returns>The details of the call.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the call does not exist.</exception>
    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            DO.Call callData;
            // Retrieve call details from the data layer
            lock (AdminManager.BlMutex)
                callData = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist.");
                // Convert DO.Call to BO.Call and return
                return CallManager.ConvertFromDoToBo(callData);

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist.", ex);
        }
    }

    /// <summary>
    /// Retrieves a list of calls with optional filtering and sorting.
    /// </summary>
    /// <param name="callFilter">Filter by the specified field.</param>
    /// <param name="filterValue">The value to filter by.</param>
    /// <param name="callSort">Sort by the specified field.</param>
    /// <returns>A list of calls.</returns>
    /// 
    public IEnumerable<BO.CallInList> GetCallList(BO.MySortInCallInList? callFilter, object? filterValue, BO.MySortInCallInList? callSort)
    {
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex)
             calls = _dal.Call.ReadAll();
        var callsBo = calls.Select(callData => CallManager.ConvertToCallInList(callData)).Distinct().ToList();

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
                        callsBo = callsBo.Where(c => c.StartTime == (DateTime)resultT).ToList();
                    break;

                case BO.MySortInCallInList.TimeRemaining:
                    if (TimeSpan.TryParse(filterValue.ToString(), out var resultTR))
                        callsBo = callsBo.Where(c => c.TimeRemaining == (TimeSpan)resultTR).ToList();
                    break;

                case BO.MySortInCallInList.CompletionTime:
                    if (TimeSpan.TryParse(filterValue.ToString(), out var resultCT))
                        callsBo = callsBo.Where(c => c.CompletionTime == (TimeSpan)resultCT).ToList();
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
        return CallManager.SortCalls(callsBo, callSort ?? default).ToList();
   
    }

    /// <summary>
    /// Assigns a volunteer to treat a specific call.
    /// </summary>
    /// <param name="idV">The ID of the volunteer.</param>
    /// <param name="idC">The ID of the call.</param>
    /// <exception cref="BO.BlInvalidOperationException">Thrown when the call is already being treated or has expired.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the call or assignment does not exist.</exception>
    public void SelectCallToTreat(int idV, int idC)
    {
        try
        {
            DO.Call? call;
            IEnumerable<DO.Assignment> assignments;
            // Retrieve call details from the data layer
            lock (AdminManager.BlMutex)
            {
                call = _dal.Call.Read(idC);
                // Retrieve all assignments for the call
                 assignments = _dal.Assignment.ReadAll();
            }
            assignments = assignments.Where(a => a.CallId == idC).ToList();

            // Check if there are any open assignments for the call
            if (assignments.Any() && CallManager.IsOpenAssignment(assignments))
                throw new BO.BlInvalidOperationException("The call is already being treated by another volunteer.");

            // Check if the call has expired
            if (AdminManager.Now > call?.MaxFinishCall)
                throw new BO.BlInvalidOperationException("The call has expired.");
            lock (AdminManager.BlMutex)
                _dal.Assignment.Create(new DO.Assignment(0, idC, idV, AdminManager.Now, null, null));
            VolunteerManager.Observers.NotifyItemUpdated(idV);
            CallManager.Observers.NotifyItemUpdated(idC);
            CallManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyListUpdated();

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call or assignment with ID {idC} does not exist.", ex);
        }
    }

    /// <summary>
    /// Sorts closed calls based on specified criteria.
    /// </summary>
    /// <param name="idV">The ID of the volunteer.</param>
    /// <param name="callType">The type of call to filter by.</param>
    /// <param name="closeCall">The criteria to sort by.</param>
    /// <returns>A list of closed calls.</returns>
    public IEnumerable<BO.ClosedCallInList> SortClosedCalls(int idV, BO.MyCallType? callType, BO.CloseCall? closeCall)
    {
        IEnumerable<DO.Assignment> closeList;
        lock (AdminManager.BlMutex)
             closeList = _dal.Assignment.ReadAll(a => a.VolunteerId == idV && a.FinishType != null);
        var closeListNew = closeList.Where(a => a.VolunteerId == idV &&a.FinishType!=null).Select(a => convertAssignmentToClosed(a)).ToList();
        closeListNew = callType.HasValue ? closeListNew.Where(call => call.Type == callType.Value).ToList() : closeListNew;
        return closeListNew;
    }


    /// <summary>
    /// Sorts opened calls based on specified criteria.
    /// </summary>
    /// <param name="idV">The ID of the volunteer.</param>
    /// <param name="callType">The type of call to filter by.</param>
    /// <param name="openedCall">The criteria to sort by.</param>
    /// <returns>A list of opened calls.</returns>
    public IEnumerable<BO.OpenCallInList> SortOpenedCalls(int idV, BO.MyCallType? callType, BO.OpenedCall? openedCall)
    {
       DO.Volunteer? volunteer;
        IEnumerable<DO.Call> openCalls;
        lock (AdminManager.BlMutex)
        {
            volunteer = _dal.Volunteer.Read(idV);
            openCalls = _dal.Call.ReadAll();
        }
       var calls= openCalls.Where(c=>CallManager.OpenCondition(c)&&CallManager.VolunteerArea(volunteer, c)).Select(c => CallManager.convertCallToOpened(volunteer,c)).ToList();
        calls = callType.HasValue ? calls.Where(call => call.Type == callType.Value).ToList() : calls;
        return CallManager.SortOpenCallsByField(calls, openedCall);

    }

    /// <summary>
    /// Updates the details of a specific call.
    /// </summary>
    /// <param name="myCall">The updated call details.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the call does not exist.</exception>
    public void UpdateCall(BO.Call myCall)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            // Validate the format of the values
            CallManager.ValidateCallFormat(myCall);
            // Validate the logical correctness of the values
            CallManager.ValidateCallLogic(myCall);
            var callDo = CallManager.ConvertBOToDO(myCall);
            DO.Call newCall;
            if (myCall.Status == BO.MyCallStatus.Open||myCall.Status==BO.MyCallStatus.OpenAtRisk)
            {
                // Attempt to update the call in the data layer
                lock (AdminManager.BlMutex)
                {
                    _dal.Call.Update(callDo);
                    newCall = _dal.Call.ReadAll().Last();
                    _ = CallManager.updateCoordinatesForCallAddressAsync(newCall);
                }

            }
            else
            {
                throw new BO.BlInvalidOperationException("You cannot update a call that has ended or is in progress.");
            }
            CallManager.Observers.NotifyItemUpdated(myCall.Id); 
            CallManager.Observers.NotifyListUpdated();  
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID {myCall.Id} does not exist.", ex);
        }
        catch (BO.BlTemporaryNotAvailableException ex)
        {
            throw new BO.BlTemporaryNotAvailableException(ex.Message);
        }
    }

    /// <summary>
    /// Cancels the treatment of a call by a volunteer.
    /// </summary>
    /// <param name="idV">The ID of the volunteer.</param>
    /// <param name="idC">The ID of the call.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown when the requester is not authorized to cancel the assignment.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown when the assignment has already been completed or canceled.</exception
    public void UpdateCancelTreatment(int idV, int idC)
    {
        try
        {
            DO.Assignment? assignment;
            // Retrieve assignment details from the data layer
            lock (AdminManager.BlMutex)
                 assignment = _dal.Assignment.Read(a => a.CallId == idC&&a.FinishType==null);
            // Check authorization: the requester must be a manager or the volunteer assigned to the task
            if (assignment?.VolunteerId != idV&& (!CallManager.IsManager(idV)))
                    throw new BO.BlUnauthorizedAccessException("The requester is not authorized to cancel this assignment.");
                    // Check that the assignment is still open and not already completed or canceled
                    if (assignment!=null&&assignment.FinishCall.HasValue ||assignment!=null&& assignment.FinishType.HasValue)
                        throw new BO.BlInvalidOperationException("The assignment has already been completed or canceled.");
                    // Create a new assignment object with the updated finish time and finish type
                    var updatedAssignment = assignment with
                    {
                        FinishCall = AdminManager.Now,
                        FinishType = assignment.VolunteerId == idV ? DO.MyFinishType.SelfCancel : DO.MyFinishType.ManagerCancel
                    };
                    // Attempt to update the assignment entity in the data layer
                    lock (AdminManager.BlMutex)
                        _dal.Assignment.Update(updatedAssignment);
                    CallManager.Observers.NotifyItemUpdated(idC);
                    VolunteerManager.Observers.NotifyItemUpdated(idV);
                    CallManager.Observers.NotifyListUpdated();
                    _ = CallManager.UpdateCancelTreatmentSendEmailAsync(idV, assignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the assignment does not exist, throw an appropriate exception to the presentation layer
            throw new BO.BlDoesNotExistException($"Assignment with ID {idC} does not exist.", ex);
        }
    }
    /// <summary>
    /// Marks an assignment as completed by a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">ID of the volunteer completing the assignment</param>
    /// <param name="assignmentId">ID of the assignment to be completed</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the volunteer is not authorized to complete the assignment</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the assignment is already completed or canceled</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the assignment does not exist</exception>
    public void UpdateCompleteAssignment(int volunteerId, int idC)
    {
        try
        {
            DO.Assignment? assignment;
            lock (AdminManager.BlMutex)
                 assignment = _dal.Assignment.Read(a => a.CallId == idC&&a.FinishType==null);

            // Check authorization: the volunteer must be the one assigned to the task
            if (assignment?.VolunteerId != volunteerId)
                throw new BO.BlUnauthorizedAccessException("The volunteer is not authorized to complete this assignment.");

            // Check that the assignment is still open and not already completed or canceled
            if (assignment.FinishCall.HasValue || assignment.FinishType.HasValue)
                throw new BO.BlInvalidOperationException("The assignment has already been closed or canceled.");

            // Create a new assignment object with the updated finish time and finish type
            var updatedAssignment = assignment with
            {
                FinishCall = AdminManager.Now,
                FinishType = DO.MyFinishType.Treated
            };

            // Attempt to update the assignment entity in the data layer
            lock (AdminManager.BlMutex)
                _dal.Assignment.Update(updatedAssignment);
            CallManager.Observers.NotifyItemUpdated(idC);
            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
            CallManager.Observers.NotifyListUpdated();

        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the assignment does not exist, throw an appropriate exception to the presentation layer
            throw new BO.BlDoesNotExistException($"Assignment with ID {idC} does not exist.", ex);
        }
    }
    public IEnumerable<CallInList> GetFilterCallList(BO.MyCallStatus filter)
    {
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex)
            calls = _dal.Call.ReadAll();
        var callList = calls.Select(c => CallManager.ConvertToCallInList(c)).ToList();
        return filter != BO.MyCallStatus.None ? callList.Where(v => v.Status == filter).ToList() : callList;
    }

    public void AddObserver(Action listObserver) =>
CallManager.Observers.AddListObserver(listObserver); 
    public void AddObserver(int id, Action observer) =>
CallManager.Observers.AddObserver(id, observer); 
    public void RemoveObserver(Action listObserver) =>
CallManager.Observers.RemoveListObserver(listObserver); 
    public void RemoveObserver(int id, Action observer) =>
CallManager.Observers.RemoveObserver(id, observer); 

}
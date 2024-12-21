using BO;

namespace BlApi;

public interface  ICall
{
    // Adds a new call to the system
    public void AddCall(BO.Call call);

// Returns an array with the count of calls per status
public int[] CallAmount();

// Deletes a call based on its ID
public void DeleteCall(int callId);

// Retrieves the details of a specific call by its ID
public BO.Call GetCallDetails(int callId);

// Retrieves a list of calls with optional filtering and sorting
public IEnumerable<BO.CallInList> GetCallList(BO.MySortInCallInList? callFilter, object? filterValue, BO.MySortInCallInList? callSort);

// Selects a call to treat by a volunteer
public void SelectCallToTreat(int idV, int idC);

// Sorts the closed calls for a specific volunteer with optional filtering and sorting
public IEnumerable<BO.ClosedCallInList> SortClosedCalls(int idV, MyCallType? callType, CloseCall? closeCall);

// Sorts the opened calls for a specific volunteer with optional filtering and sorting
public IEnumerable<BO.OpenCallInList> SortOpenedCalls(int idV, MyCallType? callType, BO.OpenedCall? openedCall);

    // Updates the details of a specific call
   public void UpdateCall(BO.Call myCall);
    // Cancels the treatment of an assignment
    public void UpdateCancelTreatment(int idV, int idC);

// Completes an assignment
public void UpdateCompleteAssignment(int volunteerId, int assignmentId);
}

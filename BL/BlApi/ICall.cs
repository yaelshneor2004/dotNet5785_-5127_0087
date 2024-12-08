namespace BlApi;

public interface  ICall
{
    int[] CallAmount(); //Returns the amount of calls

    //Returns a sorted and filtered collection of a logical data entity
    BO.CallInList GetCallList(BO.CallInList? CallFilterBy, Object? obj, BO.CallInList? CallSortBy);

    //Returns the object it constructed
    BO.Volunteer GetCallDetails(int callId);
    void UpdateCallDetails(BO.Call call);

    void DeleteCall(int callId);

    void AddCall(BO.Call myCall);


    IEnumerable<BO.ClosedCallInList> SortClosedCalls(int idV, BO.MyCallType? callType);  

    IEnumerable<BO.OpenCallInList> SortOpenedCalls(int idV, BO.MyCallType? callType,BO.OpenCallInList openCallInList);

    void UpdateEndTreatment(int idV, int idC);
    void UpdateCancelTreatment(int idV, int idC);
    void SelectCallToTreat(int idV, int idC);



    /**/

}

using BlApi;
using BO;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    public void AddCall(Call myCall)
    {
        throw new NotImplementedException();
    }

    public int[] CallAmount()
    {
        throw new NotImplementedException();
    }

    public void DeleteCall(int id)
    {
        throw new NotImplementedException();
    }

    public Volunteer GetCallDetails(int id)
    {
        throw new NotImplementedException();
    }

    public CallInList GetCallList(CallInList? CallFilterBy, object? obj, CallInList? CallSortBy)
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

    public void UpdateCall(Volunteer myVolunteer)
    {
        throw new NotImplementedException();
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

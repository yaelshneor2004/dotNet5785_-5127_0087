using System.Collections;
namespace PL;

internal class SortCollectionCallInList : IEnumerable
{
    static readonly IEnumerable<BO.MySortInCallInList> s_enums =
        (Enum.GetValues(typeof(BO.MySortInCallInList)) as IEnumerable<BO.MySortInCallInList>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class SortCollectionMyCallType : IEnumerable
{
    static readonly IEnumerable<BO.MyCallType> s_enums =
        (Enum.GetValues(typeof(BO.MyCallType)) as IEnumerable<BO.MyCallType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class SortCollectionRoles : IEnumerable
{
    static readonly IEnumerable<BO.MyRole> s_enums =
        (Enum.GetValues(typeof(BO.MyRole)) as IEnumerable<BO.MyRole>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class SortCollectionDistances : IEnumerable
{
    static readonly IEnumerable<BO.MyTypeDistance> s_enums =
        (Enum.GetValues(typeof(BO.MyTypeDistance)) as IEnumerable<BO.MyTypeDistance>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class SortCollectionCallStatusByVolunteer : IEnumerable
{
    static readonly IEnumerable<BO.MyCallStatusByVolunteer> s_enums =
        (Enum.GetValues(typeof(BO.MyCallStatusByVolunteer)) as IEnumerable<BO.MyCallStatusByVolunteer>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}


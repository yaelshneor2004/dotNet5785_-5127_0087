using System.Collections;
namespace PL;

/// <summary>
/// Enumerator for sorting call-in list.
/// This class implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class SortCollectionCallInList : IEnumerable
{
    static readonly IEnumerable<BO.MyCallStatus> s_enums =
        (Enum.GetValues(typeof(BO.MyCallStatus)) as IEnumerable<BO.MyCallStatus>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// Enumerator for sorting call types.
/// This class implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class SortCollectionMyCallType : IEnumerable
{
    static readonly IEnumerable<BO.MyCallType> s_enums =
        (Enum.GetValues(typeof(BO.MyCallType)) as IEnumerable<BO.MyCallType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// Enumerator for sorting roles.
/// This class implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class SortCollectionRoles : IEnumerable
{
    static readonly IEnumerable<BO.MyRole> s_enums =
        (Enum.GetValues(typeof(BO.MyRole)) as IEnumerable<BO.MyRole>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// Enumerator for sorting distances.
/// This class implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class SortCollectionDistances : IEnumerable
{
    static readonly IEnumerable<BO.MyTypeDistance> s_enums =
        (Enum.GetValues(typeof(BO.MyTypeDistance)) as IEnumerable<BO.MyTypeDistance>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// Enumerator for sorting call statuses by volunteer.
/// This class implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class SortCollectionCallStatusByVolunteer : IEnumerable
{
    static readonly IEnumerable<BO.MyCallStatusByVolunteer> s_enums =
        (Enum.GetValues(typeof(BO.MyCallStatusByVolunteer)) as IEnumerable<BO.MyCallStatusByVolunteer>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}


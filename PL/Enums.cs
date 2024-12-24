using System.Collections;
namespace PL;

    internal class SortCollectionCallInList : IEnumerable
    {
        static readonly IEnumerable<BO.MySortInCallInList> s_enums =
    (Enum.GetValues(typeof(BO.MySortInCallInList)) as IEnumerable<BO.MySortInCallInList>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class SortCollectionVolunteersInList : IEnumerable
    {
        static readonly IEnumerable<BO.MyCallType> s_enums =
    (Enum.GetValues(typeof(BO.MyCallType)) as IEnumerable<BO.MyCallType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

using System.Collections;
namespace PL;

public class Enums
{
    internal class SortCollectionCallInList : IEnumerable
    {
        static readonly IEnumerable<BO.MySortInCallInList> s_enums =
    (Enum.GetValues(typeof(BO.MySortInCallInList)) as IEnumerable<BO.MySortInCallInList>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

}


namespace Dal;

internal static class Config
{
    internal const int startCallId = 1;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId++; }

    internal const int startAssignmentId = 1;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { get => nextCallId++; }

    internal static DateTime Clock { get; set; } = DateTime.Now;


    internal static TimeSpan RiskRange { get;set; }=TimeSpan.FromDays(14);
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;
        RiskRange= TimeSpan.FromDays(14);


    }
}

using BO;

namespace BlApi;

public interface  IAdmin
{
    DateTime AdvanceClock(Clock advance);
    DateTime GetClock();
    TimeSpan GetRiskRange();
    void SetRiskRange(TimeSpan time);
    void Initialization();
    void Reset();
}

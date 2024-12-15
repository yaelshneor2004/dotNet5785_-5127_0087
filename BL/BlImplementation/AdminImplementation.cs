using BlApi;
using BO;
using DalApi;
using DalTest;
using Helpers;
namespace BlImplementation;
internal class AdminImplementation : IAdmin
{
    private static IDal _dal = DalApi.Factory.Get;
    public DateTime AdvanceClock(Clock advance)
    {
        switch (advance)
        {
            case Clock.Minute:
                ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1)); 

                break;
            case Clock.Hour:
                ClockManager.UpdateClock(ClockManager.Now.AddHours(1));

                break;
            case Clock.Day:
                ClockManager.UpdateClock(ClockManager.Now.AddDays(1));
                break;
            case Clock.Month:
                ClockManager.UpdateClock(ClockManager.Now.AddMonths(1));

                break;
            case Clock.Year:
                ClockManager.UpdateClock(ClockManager.Now.AddYears(1));
                break;
            default:
                throw new Exception("invalid choice");
        }
        ClockManager.UpdateClock(_dal.Config.Clock);
        return _dal.Config.Clock;
    }
    public DateTime GetClock()
    {
        return _dal.Config.Clock;
    }
    public TimeSpan GetRiskRange()
    {
        return _dal.Config.RiskRange;

    }
    public void initialization()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(GetClock());
    }
    public void Reset()
    {
        _dal.ResetDB();
    }
    public void SetRiskRange(TimeSpan time)
    {
        _dal.Config.RiskRange = time;
    }
}


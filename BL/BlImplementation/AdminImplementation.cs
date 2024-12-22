using BlApi;
using DalApi;
using DalTest;
using Helpers;
namespace BlImplementation;
internal class AdminImplementation : IAdmin
{
    private static IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Advances the system clock by a specified amount of time.
    /// </summary>
    /// <param name="advance">The amount of time to advance the clock by.</param>
    /// <returns>The updated clock time.</returns>
    /// <exception cref="Exception">Thrown if an invalid time unit is specified.</exception>
    public DateTime AdvanceClock(BO.Clock advance)
    {
        switch (advance)
        {
            case BO.Clock.Minute:
                ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));
                break;
            case BO.Clock.Hour:
                ClockManager.UpdateClock(ClockManager.Now.AddHours(1));
                break;
            case BO.Clock.Day:
                ClockManager.UpdateClock(ClockManager.Now.AddDays(1));
                break;
            case BO.Clock.Month:
                ClockManager.UpdateClock(ClockManager.Now.AddMonths(1));
                break;
            case BO.Clock.Year:
                ClockManager.UpdateClock(ClockManager.Now.AddYears(1));
                break;
            default:
                throw new Exception("invalid choice");
        }
        ClockManager.UpdateClock(_dal.Config.Clock);
        return _dal.Config.Clock;
    }

    /// <summary>
    /// Retrieves the current system clock time.
    /// </summary>
    /// <returns>The current clock time.</returns>
    public DateTime GetClock()
    {
        return _dal.Config.Clock;
    }

    /// <summary>
    /// Retrieves the current risk range time span.
    /// </summary>
    /// <returns>The current risk range time span.</returns>
    public TimeSpan GetRiskRange()
    {
        return _dal.Config.RiskRange;
    }

    /// <summary>
    /// Initializes the system by performing necessary setup operations.
    /// </summary>
    public void Initialization()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(GetClock());
    }

    /// <summary>
    /// Resets the database to its initial state.
    /// </summary>
    public void Reset()
    {
        _dal.ResetDB();
    }

    /// <summary>
    /// Sets the risk range time span.
    /// </summary>
    /// <param name="time">The new risk range time span.</param>
    public void SetRiskRange(TimeSpan time)
    {
        _dal.Config.RiskRange = time;
    }
}
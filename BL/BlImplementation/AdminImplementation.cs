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
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
                break;
            case BO.Clock.Hour:
                AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
                break;
            case BO.Clock.Day:
                AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
                break;
            case BO.Clock.Month:
                AdminManager.UpdateClock(AdminManager.Now.AddMonths(1));
                break;
            case BO.Clock.Year:
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
                break;
            default:
                throw new Exception("invalid choice");
        }
        AdminManager.UpdateClock(AdminManager.Clock);
        return AdminManager.Clock;
    }

    /// <summary>
    /// Retrieves the current system clock time.
    /// </summary>
    /// <returns>The current clock time.</returns>
    public DateTime GetClock()
    {
        return AdminManager.Clock;
    }

    /// <summary>
    /// Retrieves the current risk range time span.
    /// </summary>
    /// <returns>The current risk range time span.</returns>
    public TimeSpan GetRiskRange()
    {
        return AdminManager.RiskRange;
    }

    /// <summary>
    /// Initializes the system by performing necessary setup operations.
    /// </summary>
    public void Initialization()
    {
        DalTest.Initialization.Do();
        AdminManager.UpdateClock(GetClock());
        //AdminManager.MaxRange = AdminManager.MaxRange;
        //AdminManagerלהוסיף פה את שאר משתני התצורה שצריך, מה שנעשה ב
    }

    /// <summary>
    /// Resets the database to its initial state.
    /// </summary>
    public void Reset()
    {
        _dal.ResetDB();
        AdminManager.UpdateClock(GetClock());
        //AdminManager.MaxRange = AdminManager.MaxRange;
        //AdminManagerלהוסיף פה את שאר משתני התצורה שצריך, מה שנעשה ב
    }

    /// <summary>
    /// Sets the risk range time span.
    /// </summary>
    /// <param name="time">The new risk range time span.</param>
    public void SetRiskRange(TimeSpan time)
    {
        AdminManager.RiskRange = time;
    }

    public void AddClockObserver(Action clockObserver) =>
AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;

}
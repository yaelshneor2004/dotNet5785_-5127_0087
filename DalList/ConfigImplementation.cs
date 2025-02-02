using DalApi;
namespace Dal;

//The ConfigImplementation class manages configuration settings, including getting and setting the clock and risk range, and resetting the configuration. 🌟
internal class ConfigImplementation : IConfig
{   public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskRange
    { 
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }
    public void Reset()
    {
        Config.Reset();
    }

}

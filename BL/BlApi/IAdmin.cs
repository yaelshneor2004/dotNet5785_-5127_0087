using BO;

namespace BlApi;

public interface IAdmin
{
    DateTime AdvanceClock(Clock advance);
    DateTime GetClock();
    TimeSpan GetRiskRange();
    void SetRiskRange(TimeSpan time);
    void Initialization();
    void Reset();
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    void StartSimulator(int interval); //stage 7
    void StopSimulator(); //stage 7

}
//using BO;

//namespace BlApi;

//public interface IAdmin
//{
//    DateTime AdvanceClock(Clock advance);
//    DateTime GetClock();
//    TimeSpan GetRiskRange();
//    void SetRiskRange(TimeSpan time);
//    void Initialization();
//    void Reset();
//    void AddConfigObserver(Action configObserver);
//    void RemoveConfigObserver(Action configObserver);
//    void AddClockObserver(Action clockObserver);
//    void RemoveClockObserver(Action clockObserver);

//}

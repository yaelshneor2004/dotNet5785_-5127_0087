namespace BlApi;
public interface IBl : IObservable
{
    ICall Call { get; }
    IVolunteer Volunteer { get; }
    IAdmin Admin { get; }
}

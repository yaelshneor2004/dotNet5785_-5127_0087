namespace BlApi;

public interface IVolunteer : IObservable
{
    //Returns the user role
    BO.MyRole Login(string username, string password);

    //Returns a sorted and filtered collection of a logical data entity
    IEnumerable< BO.VolunteerInList> GetVolunteerList(bool? IsActive, BO.MySortInVolunteerInList? mySortInVolunteerInList);

    //Returns the object it constructed
    BO.Volunteer GetVolunteerDetails(int id);

   void UpdateVolunteer(int id, BO.Volunteer myVolunteer); //update

    void DeleteVolunteer(int volunteerId);

    void AddVolunteer(BO.Volunteer myVolunteer);


}

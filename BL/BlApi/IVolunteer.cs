

namespace BlApi;

public interface IVolunteer
{
    //Returns the user role
    BO.MyRole Login(string username, string password);

    //Returns a sorted and filtered collection of a logical data entity
    BO.VolunteerInList GetVolunteerList(bool? IsActive, BO.MyCurrentCallType? myCurrentCallType);

    //Returns the object it constructed
    BO.Volunteer GetVolunteerDetails(int id);

   void UpdateVolunteer(int id, BO.Volunteer myVolunteer); //update

    void DeleteVolunteer( int id );

    void AddVolunteer(BO.Volunteer myVolunteer);


}

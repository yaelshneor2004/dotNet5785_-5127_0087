
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;


public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (Read(item.Id) != null)
        {
            throw new("An object of type Volunteer with such an ID already exists");
        }
        DataSource.Volunteers.Add(item);

    }


    public void Delete(int id)
    {
        for (int i = 0; i < DataSource.Volunteers.Count; i++)
        {
            if (DataSource.Volunteers[i].Id == id)
                DataSource.Volunteers.Remove(DataSource.Volunteers[i]);
        }
        throw new("An object of type Volunteer with such an ID does not exist");
    }


    public void DeleteAll()
    {
       DataSource.Volunteers.Clear();
    }


    public Volunteer? Read(int id)
    {
        for (int i = 0; i < DataSource.Volunteers.Count; i++)
        {
            if (DataSource.Volunteers[i].Id == id)
                return DataSource.Volunteers[i];
        }
        return null;
    }


    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        for (int i = 0; i < DataSource.Volunteers.Count; i++)
        {
            if (DataSource.Volunteers[i].Id == item.Id)
            {
                DataSource.Volunteers.Remove(DataSource.Volunteers[i]);
                DataSource.Volunteers.Add(item);
            }

        }
        throw new ("An object of type Volunteer with such an ID does not exist");
    }
    
}

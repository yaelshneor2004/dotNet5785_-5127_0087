
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        //for entities with auto id
        int newId = Config.NextAssignmentId;
        int new2Id = Config.NextAssignmentId;
        Assignment newAssignment = item with { Id = newId,CallId=new2Id };
        DataSource.Assignments.Add(newAssignment);
        
    }

    public void Delete(int id)
    {
        for (int i = 0; i < DataSource.Assignments.Count; i++)
        {
            if (DataSource.Assignments[i].Id == id)
                DataSource.Assignments.Remove(DataSource.Assignments[i]);
        }
        throw new Exception($"An object of type Assignment with such an ID={id} does not exist");
    }


    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }


    public Assignment? Read(int id)
    {
        for (int i = 0; i < DataSource.Assignments.Count; i++)
        {
            if (DataSource.Assignments[i].Id == id)
                return DataSource.Assignments[i];
        }
        return null;
    }


    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        for (int i = 0; i < DataSource.Assignments.Count; i++)
        {
            if (DataSource.Assignments[i].Id == item.Id)
            {
                DataSource.Assignments.Remove(DataSource.Assignments[i]);
                DataSource.Assignments.Add(item);
            }

        }
        throw new Exception($"An object of type Assignment with such an ID={item.Id} does not exist");
    }

    
}
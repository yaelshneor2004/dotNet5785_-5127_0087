namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

// The AssignmentImplementation class manages operations for Assignment entities, including creating, reading, updating, and deleting assignments in the data source.
internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        // For entities with auto-generated ID
        int newId = Config.NextAssignmentId;
        int new2Id = Config.NextAssignmentId;
        Assignment newAssignment = item with { Id = newId, CallId = new2Id };
        DataSource.Assignments.Add(newAssignment);
    }

    public void Delete(int id)
    {
        var assignment = Read(id);
        if (assignment != null)
        {
            DataSource.Assignments.Remove(assignment);
        }
        else
        {
            throw new DalDeletionImpossible($"An object of type Assignment with such an ID={id} does not exist");
        }
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(item => item.Id == id);
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return filter == null
            ? DataSource.Assignments
            : DataSource.Assignments.Where(filter);
    }

    public void Update(Assignment item)
    {
        var existingAssignment = Read(item.Id);
        if (existingAssignment != null)
        {
            DataSource.Assignments.Remove(existingAssignment);
            DataSource.Assignments.Add(item);
        }
        else
        {
            throw new DalDoesNotExistException($"An object of type Assignment with such an ID={item.Id} does not exist");
        }
    }
}

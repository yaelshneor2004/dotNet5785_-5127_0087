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
        if (DataSource.Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDeletionImpossible($"An object of type Assignment with such an ID={id} does not exist");
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

    //retrieves all Assignment entities, optionally filtering them based on a provided condition
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return filter == null
            ? DataSource.Assignments
            : DataSource.Assignments.Where(filter);
    }

    public void Update(Assignment item)
    {
        if (DataSource.Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"An object of type Assignment with such an ID={item.Id} does not exist");
        DataSource.Assignments.Add(item);
    }
}

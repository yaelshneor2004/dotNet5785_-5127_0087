namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

// The CallImplementation class manages operations for Call entities, including creating, reading, updating, and deleting calls in the data source.
internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        // For entities with auto-generated ID
        int newId = Config.NextCallId;
        Call newCall = item with { Id = newId };
        DataSource.Calls.Add(newCall);
    }

    public void Delete(int id)
    {
        var call = Read(id);
        if (call != null)
        {
            DataSource.Calls.Remove(call);
        }
        else
        {
            throw new DalDeletionImpossible($"An object of type Call with such an ID={id} does not exist");
        }
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(item => item.Id == id);
    }

    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        return filter == null
            ? DataSource.Calls
            : DataSource.Calls.Where(filter);
    }

    public void Update(Call item)
    {
        var existingCall = Read(item.Id);
        if (existingCall != null)
        {
            DataSource.Calls.Remove(existingCall);
            DataSource.Calls.Add(item);
        }
        else
        {
            throw new DalDoesNotExistException($"An object of type Call with such an ID={item.Id} does not exist");
        }
    }
}

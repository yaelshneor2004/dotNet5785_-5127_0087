namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

// The CallImplementation class manages operations for Call entities, including creating, reading, updating, and deleting calls in the data source.
internal class CallImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        // For entities with auto-generated ID
        int newId = Config.NextCallId;
        Call newCall = item with { Id = newId };
        DataSource.Calls.Add(newCall);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
        {
            if (DataSource.Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDeletionImpossible($"An object of type Call with such an ID={id} does not exist");
       }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(item => item.Id == id);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    //retrieves all Call entities, optionally filtering them based on a provided condition
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        return filter == null
            ? DataSource.Calls
            : DataSource.Calls.Where(filter);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
            if (DataSource.Calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"An object of type Call with such an ID={item.Id} does not exist");
        int nextId = Config.NextCallId;
        Call newCall = item with { Id = nextId };
        DataSource.Calls.Add(newCall);
      }
    
}

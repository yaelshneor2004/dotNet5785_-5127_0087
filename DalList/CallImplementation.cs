

namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall

{
    public void Create(Call item)
    {
        //for entities with auto id
        int newId = Config.NextCallId;
        Call newCall = item with { Id = newId };
        DataSource.Calls.Add(newCall);
    }


    public void Delete(int id)
    {
        for (int i = 0; i < DataSource.Calls.Count; i++)
        {
            if (DataSource.Calls[i].Id == id)
                DataSource.Calls.Remove(DataSource.Calls[i]);
        }
        throw new Exception($"An object of type Call with such an ID={id} does not exist");
    }


    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }


    public Call? Read(int id)
    {
        for (int i = 0; i < DataSource.Calls.Count; i++)
        {
            if (DataSource.Calls[i].Id == id)
                return DataSource.Calls[i];
        }
        return null;
    }


    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        for (int i = 0; i < DataSource.Calls.Count; i++)
        {
            if (DataSource.Calls[i].Id == item.Id)
            {
                DataSource.Calls.Remove(DataSource.Calls[i]);
                DataSource.Calls.Add(item);
            }

        }
        throw new Exception($"An object of type Call with such an ID={item.Id} does not exist");
    }
}

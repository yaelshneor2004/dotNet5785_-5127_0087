
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class CallImplementation : ICall
{


    public void Update(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        int nextId = Config.NextCallId;
        Call newCall = item with { Id = nextId };
        Calls.Add(newCall);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }

    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

    public void Create(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
       int nextId=Config.NextCallId;
        Call copy=item with { Id = nextId };
        Calls.Add(copy);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);

    }

    public Call? Read(Func<Call, bool> filter)
    {
        return XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml).FirstOrDefault(filter);
    }

    public Call? Read(int id)
    {
        return XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml).FirstOrDefault(it => it.Id == id);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return (filter == null
                  ? Calls
                  : Calls.Where(filter));
    }

}

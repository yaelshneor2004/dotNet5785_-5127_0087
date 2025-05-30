﻿
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

//The VolunteerImplementation class manages operations for Volunteer entities, including creating, reading, updating, and deleting calls in the data source
internal class VolunteerImplementation : IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        if (Read(item.Id) != null)
        {
            throw new DalAlreadyExistsException($"An object of type Volunteer with such an ID={item.Id} already exists");
        }
        DataSource.Volunteers.Add(item);

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id) 
    {
            if (DataSource.Volunteers.RemoveAll(it => it.Id == id) == 0)
            throw new DalDeletionImpossible($"An object of type Volunteer with such an ID={id} does not exist");
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
       DataSource.Volunteers.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(item => item.Id == id); 
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }
    //retrieves all Volunteer entities, optionally filtering them based on a provided condition
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) 
               => filter == null
                   ? DataSource.Volunteers.Select(item => item)
                   : DataSource.Volunteers.Where(filter);
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item) 
    {
            if (DataSource.Volunteers.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"An object of type Volunteer with such an ID={item.Id} does not exist");
        DataSource.Volunteers.Add(item);
     
    }
}

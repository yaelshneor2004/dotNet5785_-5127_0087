﻿using DO;

namespace DalApi;

public interface ICrud<T> where T : class
{
    T? Read(Func<T, bool> filter); //get obj by any parameter
    void Create(T item); //Creates new entity object in DAL
    T? Read(int id); //Reads entity object by its ID 
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null); // stage 2 
    void Update(T item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
}

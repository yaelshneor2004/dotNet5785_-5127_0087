﻿namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    static Assignment getAssignment(XElement s)
    {
        return new DO.Assignment()
        {
            Id = s.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            CallId = s.ToIntNullable("CallId") ?? throw new FormatException("can't convert CallId"),
            VolunteerId = s.ToIntNullable("VolunteerId") ?? throw new FormatException("can't convert VolunteerId"),
            StartCall = s.ToDateTimeNullable("StartCall") ?? throw new FormatException("can't convert startCall"),
            FinishCall = s.ToDateTimeNullable("FinishCall"),
            FinishType = s.ToEnumNullable<MyFinishType>("FinishType")
        };
    }

   
    [MethodImpl(MethodImplOptions.Synchronized)]
    private IEnumerable<XElement>GetAssignmentElement(Assignment assignment)
    {
        yield return new XElement("Id", assignment.Id);
        yield return new XElement("CallId", assignment.CallId);
        yield return new XElement("VolunteerId", assignment.VolunteerId);
        yield return new XElement("StartCall", assignment.StartCall);
        yield return new XElement("FinishType", assignment.FinishType?.ToString()); // Convert Enum to string
        yield return new XElement("FinishCall", assignment.FinishCall);
    }
    //Converts an Assignment object into an XElement
    [MethodImpl(MethodImplOptions.Synchronized)]
    private XElement createAssignmentElement(Assignment assignment)
    {
        return new XElement("Assignment", GetAssignmentElement(assignment));
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        XElement? AssignmenttElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return AssignmenttElem is null ? null : getAssignment(AssignmenttElem);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(s => getAssignment(s)).FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    //Updates an Assignment object in the XML file by removing the old element and adding the updated one.
    public void Update(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        XElement? assignmentElem = assignmentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id);
        if (assignmentElem == null)
            throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does not exist");

        assignmentElem.Remove();

        assignmentsRootElem.Add(createAssignmentElement(item));

        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        int nextId = Config.NextAssignmentId;
        Assignment copy = item with { Id = nextId };
        assignmentsRootElem.Add(new XElement(createAssignmentElement(copy)));
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);

    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        (assignmentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={id} does Not exist"))
                .Remove();
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        assignmentsRootElem.RemoveAll();
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);

    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        return (filter == null
                   ? assignmentsRootElem.Elements().Select(s => getAssignment(s))
                   : assignmentsRootElem.Elements().Select(s => getAssignment(s)).Where(filter));
    }
}

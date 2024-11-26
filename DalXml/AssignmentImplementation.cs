
namespace Dal;


using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    static Assignment getAssignment(XElement a)
    {
        return new DO.Assignment()
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            CallId = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            VolunteerId = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            StartCall = a.ToDateTimeNullable("StartCall")?? throw new FormatException("can't convert start Call"),
            FinishCall = a.ToDateTimeNullable("FinishCall"),
            FinishType = a.ToEnumNullable<MyFinishType>("FinishType"),
        };
    }
    //Converts an Assignment object into an XElement
    static XElement createAssignmentElement(Assignment assignment)
    {
        return new XElement("Assignment",
            new XElement("Id", assignment.Id),
            new XElement("CallId", assignment.CallId),
            new XElement("VolunteerId", assignment.VolunteerId),
            new XElement("StartCall", assignment.StartCall),
            new XElement("FinishType", assignment.FinishType?.ToString()), // Convert Enum to string
            new XElement("FinishCall", assignment.FinishCall)
        );
    }


    public Assignment? Read(int id)
    {
        XElement? assignmentElem =
    XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return assignmentElem is null ? null : getAssignment(assignmentElem);
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(s => getAssignment(s)).FirstOrDefault(filter);
    }

    //Updates an Assignment object in the XML file by removing the old element and adding the updated one.
    public void Update(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        (assignmentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist"))
                .Remove();
        int nextId = Config.NextAssignmentId;
        Assignment copy = item with { Id = nextId };
        assignmentsRootElem.Add(new XElement("Assignment", createAssignmentElement(copy)));
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }


    public void Create(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        int nextId = Config.NextAssignmentId;
        Assignment copy = item with { Id = nextId };
        assignmentsRootElem.Add(new XElement("Assignment", createAssignmentElement(copy)));
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);

    }

    public void Delete(int id)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        (assignmentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={id} does Not exist"))
                .Remove();
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }

    public void DeleteAll()
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        assignmentsRootElem.RemoveAll();
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);

    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        return (filter == null
                   ? assignmentsRootElem.Elements().Select(s => getAssignment(s))
                   : assignmentsRootElem.Elements().Select(s => getAssignment(s)).Where(filter));
    }

}

namespace DalApi;
using System.Xml.Linq;
static class DalConfig
{
    /// <summary>
    /// internal PDS class
    /// </summary>
    internal record DalImplementation
    (string Package,   // package/dll name (DalList/DalXml)
     string Namespace, // namespace where DAL implementation class is contained in(dal)
     string Class   // DAL implementation class name (DalList/DalXml)
    );
    //(list or xml)
    internal static string s_dalName;
    /// <summary>hash table of DAL packages</summary>
    internal static Dictionary<string, DalImplementation> s_dalPackages;

    static DalConfig()
    {
        // load root of dal-config.xml file
        XElement dalConfig = XElement.Load(@"..\xml\dal-config.xml") ??
  throw new DalConfigException("dal-config.xml file is not found");
        //(return list or xml)
        s_dalName =
           dalConfig.Element("dal")?.Value ?? throw new DalConfigException("<dal> element is missing");
        //(returm <list>DalList</list> < xml > DalXml </ xml >)
        var packages = dalConfig.Element("dal-packages")?.Elements() ??
  throw new DalConfigException("<dal-packages> element is missing");
      
        s_dalPackages = (from item in packages
                         let pkg = item.Value
                         let ns = item.Attribute("namespace")?.Value ?? "Dal"
                         let cls = item.Attribute("class")?.Value ?? pkg
                         select (item.Name, new DalImplementation(pkg, ns, cls))
                        ).ToDictionary(p => "" + p.Name, p => p.Item2);
    }
}

[Serializable]
public class DalConfigException : Exception
{
    public DalConfigException(string msg) : base(msg) { }
    public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
}

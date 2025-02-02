namespace DalApi;
public static class Factory
{
   // To load and initialize the correct class xml or list 
    public static IDal Get
    {
        get
        {//(return list or xml)
            string dalType = DalApi.DalConfig.s_dalName ?? throw new DalConfigException($"DAL name is not extracted from the configuration");
            //(return  implementation of list or xml)
            DalApi.DalConfig.DalImplementation dal = DalApi.DalConfig.s_dalPackages[dalType] ?? throw new DalConfigException($"Package for {dalType} is not found in packages list in dal-config.xml");
            //dal.Package= (DalXml.dll or DalList.dll)
            try { System.Reflection.Assembly.Load(dal.Package ?? throw new DalConfigException($"Package {dal.Package} is null")); }
            catch (Exception ex) { throw new DalConfigException($"Failed to load {dal.Package}.dll package", ex); }
            //return reflection  of  Dal.DalList or Dal.DalXml
            Type type = Type.GetType($"{dal.Namespace}.{dal.Class}, {dal.Package}") ??
                throw new DalConfigException($"Class {dal.Namespace}.{dal.Class} was not found in {dal.Package}.dll");
            //Return the single instance of the singleton
            return type.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null) as IDal ??
                throw new DalConfigException($"Class {dal.Class} is not a singleton or wrong property name for Instance");
        }
    }
}

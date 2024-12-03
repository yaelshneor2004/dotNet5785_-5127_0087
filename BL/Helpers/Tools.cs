
using DalApi;
using System.Reflection;
using System.Text;

namespace Helpers;

internal static class Tools
{
    private static IDal s_dal = Factory.Get;
    public static string ToStringProperty<T>(this T t)
    {
            if (t == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            Type type = t.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(t, null);

                if (value is System.Collections.IEnumerable && !(value is string))
                {
                    sb.AppendLine($"{property.Name} = [");
                    foreach (var item in (System.Collections.IEnumerable)value)
                    {
                        sb.AppendLine($"  {item},");
                    }
                    sb.AppendLine("]");
                }
                else
                {
                    sb.AppendLine($"{property.Name} = {value}");
                }
            }

            return sb.ToString();
    }
}

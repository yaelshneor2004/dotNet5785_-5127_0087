using System.Globalization;
using System.Windows.Data;

namespace PL;
    public class ConvertObjIdToTF : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && int.TryParse(value.ToString(), out int id))
            {
                // If the id is 0, return false
                if (id == 0)
                {
                    return false;
                }
                return true;
            }

            // If the value is null or cannot be parsed as an integer, return false
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
public class ConvertRoleToTF : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && Enum.TryParse(value.ToString(), out BO.MyRole role))
        {
            // Directly return the result of the comparison
            return role == BO.MyRole.Manager;
        }

        // Return false if the value could not be parsed to a BO.Roles
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
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
/// <summary>
/// Converts a role to a boolean value indicating if the role is Manager.
/// </summary>
/// <param name="value">The role to convert.</param>
/// <param name="targetType">The target type.</param>
/// <param name="parameter">The converter parameter.</param>
/// <param name="culture">The culture to use in the converter.</param>
/// <returns>True if the role is Manager, otherwise false.</returns>
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

    /// <summary>
    /// Not implemented.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

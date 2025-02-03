using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

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
public class ConvertIdCalltoVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && int.TryParse(value.ToString(), out int id))
        {
            // If the id is 0, return false
            if (id == 0)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;

        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ConvertObjIdToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && int.TryParse(value.ToString(), out int id))
        {
            return id == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ConvertCallInProgressToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.Call currentCall)
        {
            // Check if the call is null or the status is expired
            if (currentCall == null || currentCall.Status == BO.MyCallStatus.Expired)
            {
                return Visibility.Collapsed;
            }

            // Otherwise, make it visible
            return Visibility.Visible;
        }

        // If the value is not a BO.CurrentCall, return Collapsed
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ConvertCallInProgressToReadOnly : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.Call currentCall)
        {
            // Check if the call is null or the status is expired
            if (currentCall == null || currentCall.Status == BO.MyCallStatus.Expired)
            {
                return true;
            }

            return false;
        }

        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ConvertVolunteerToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.Volunteer volunteer)
        {
            // Check if the volnteer is null or the current call is null
            if (volunteer == null || volunteer.CurrentCall == null)
            {
                return Visibility.Collapsed;
            }

            // Otherwise, make it visible
            return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ConvertVolunteerToReadOnly : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.Volunteer volunteer)
        {
            // Check if the volnteer is null or the current call is null
            if (volunteer == null || volunteer.CurrentCall == null)
            {
                return true;
            }

            return false;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ConvertCallToReadOnly : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.Call call)
        {
            if (call.Status==BO.MyCallStatus.Open|| call.Status == BO.MyCallStatus.OpenAtRisk)
            {
                return false;
            }

            return true;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ConvertCallComboboxToReadOnly : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.Call call)
        {
            if (call.Status == BO.MyCallStatus.Open || call.Status == BO.MyCallStatus.OpenAtRisk)
            {
                return true;
            }

            return false;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ConvertCallToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.Call call)
        {
            if (call.Status == BO.MyCallStatus.Open || call.Status == BO.MyCallStatus.OpenAtRisk)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ConvertMaxEndToReadOnly : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.Call call)
        {
            if (call.Status == BO.MyCallStatus.Closed || call.Status == BO.MyCallStatus.Expired)
            {
                return true;
            }

            return false;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ConvertCancalAssignmentToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.MyCallStatus Status)
        {
            if (Status == BO.MyCallStatus.InProgress || Status == BO.MyCallStatus.InProgressAtRisk)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }
  
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}



public class ConvertDeleteAssignmentToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Visibility.Hidden; // or any default value
        }

        BO.CallInList currentCall = (BO.CallInList)value;
        return (currentCall.Status == BO.MyCallStatus.Open || currentCall.Status == BO.MyCallStatus.OpenAtRisk) && currentCall.TotalAssignments == 0 ? Visibility.Visible : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
    public class ConvertEyeWithLine : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? TextDecorations.Underline : TextDecorations.Strikethrough;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ConvertEyeColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //return (bool)value ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
        return (bool)value ? Brushes.Black : Brushes.Gray;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ShowPassword : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Visibility.Visible : Visibility.Hidden;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class HidePasswordDots : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Visibility.Hidden : Visibility.Visible;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
    public class ActiveCallToVisibleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && value is bool && (bool)value)
        {
            return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class BooleanToReadOnlyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool booleanValue)
        {
            return booleanValue ? true : false;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class BooleanToIsEnabeldyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool booleanValue)
        {
            return booleanValue ? false : true;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
class ConvertVisibilityDeleteVol : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Visibility.Hidden; // or some default value
        }

        BO.VolunteerInList currentVol = (BO.VolunteerInList)value;
        return currentVol.CurrentCallId == null && currentVol.TotalCallsHandled == 0 ? Visibility.Visible : Visibility.Hidden;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ConvertLabelDescriptionToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class CallInProgressToAlignmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.Call currentCall)
        {
            if (currentCall == null || currentCall.Status == BO.MyCallStatus.Expired)
            {
                return HorizontalAlignment.Center;
            }
        }
        return HorizontalAlignment.Stretch;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ConvertObjIdTovisNotVis : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.CallInProgress currentCall)
        {
            if (currentCall!=null)
            {
                return Visibility.Visible;
            }
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ConvertZeroDimensionsToRed : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double dimensions)
        {
            if (dimensions == 0)
            {
                return Brushes.Red;
            }
        }
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9370DB"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}





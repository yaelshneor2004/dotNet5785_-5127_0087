using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PL;

internal class idNumValidation : ValidationRule

{
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
        string id = (string)value;
        if (!id.All(char.IsDigit))
            return new ValidationResult(false, "ID must contain only digits");
        if (id.Length != 9)
            return new ValidationResult(false, "ID must contain 9 digits");
        return ValidationResult.ValidResult;
    }
}
internal class phoneValidation : ValidationRule
{
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
        string phone = (string)value;
        if (!phone.All(char.IsDigit))
            return new ValidationResult(false, "Phone number must contain only digits");
        if (phone.Length != 10)
            return new ValidationResult(false, "Phone number must contain 10 digits");
        return ValidationResult.ValidResult;
    }
}
internal class emailValidation : ValidationRule
{
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
        string email = (string)value;
        if (!email.Contains("@"))
            return new ValidationResult(false, "Email must contain '@'");
        if (!email.Contains("."))
            return new ValidationResult(false, "Email must contain '.'");
        return ValidationResult.ValidResult;
    }
}
internal class passwordValidation : ValidationRule
{
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
        string password = (string)value;
        if (password.Length < 6)
            return new ValidationResult(false, "Password must contain at least 6 characters");
        return ValidationResult.ValidResult;
    }
}
internal class nameValidation : ValidationRule
{
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
        string name = (string)value;
        if (name.Length < 2)
            return new ValidationResult(false, "Name must contain at least 2 characters");
        return ValidationResult.ValidResult;
    }
}
internal class addressValidation : ValidationRule
{
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
        string address = (string)value;
        if (address.Length < 2)
            return new ValidationResult(false, "Address must contain at least 2 characters");
        return ValidationResult.ValidResult;
    }
}
internal class maxDistanceValidation : ValidationRule
{
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
        string maxDistance = (string)value;
        if (!maxDistance.All(char.IsDigit))
            return new ValidationResult(false, "Max distance must contain only digits");
        return ValidationResult.ValidResult;
    }
}
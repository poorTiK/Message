using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Message.ValidationRules
{
    class Login_ValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                if (Regex.IsMatch(value.ToString(), @"^(?=.*[A-Za-z0-9]$)[A-Za-z][A-Za-z\d.-]{0,19}$", RegexOptions.IgnoreCase))
                {
                    return new ValidationResult(true, null);
                }
            }
            return new ValidationResult(false, "\"Login123\"");
        }
    }
}

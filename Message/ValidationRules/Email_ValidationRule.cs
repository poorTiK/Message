using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Message.ValidationRules
{
    class Email_ValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                if (Regex.IsMatch(value.ToString(), @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                {
                    return new ValidationResult(true, null);
                }
            }
            return new ValidationResult(false, "\"example@domain.com\"");
        }
    }
}

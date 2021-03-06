﻿using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Message.ValidationRules
{
    internal class NameSurname_ValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object ob, CultureInfo cultureInfo)
        {
            if (ob != null)
            {
                if (Regex.IsMatch(ob.ToString(), @"^[A-Za-z]+$", RegexOptions.IgnoreCase))
                {
                    return new ValidationResult(true, null);
                }
            }

            return new ValidationResult(false, "Field must be filled");
        }
    }
}
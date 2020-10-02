using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LeagueAPI.Attributes
{
    public class StringInArrayValidationAttribute : ValidationAttribute
    {
        public string[] Values { get; set; }

        public override bool IsValid(object value)
        {
            var stringValue = value as string;

            return Values.Contains(stringValue);
        }
    }
}

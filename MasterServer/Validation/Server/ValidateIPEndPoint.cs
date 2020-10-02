using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MasterServer
{
    public class ValidateIPEndPoint : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!IPEndPoint.TryParse((string)value, out _))
            {
                return new ValidationResult("EndPoint must have valid syntax");
            }
            return ValidationResult.Success;
        }
    }
}

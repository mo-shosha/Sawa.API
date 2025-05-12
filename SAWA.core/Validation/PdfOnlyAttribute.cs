using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Validation
{
    public class PdfOnlyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file == null)
                return ValidationResult.Success;

            if (file.ContentType != "application/pdf")
                return new ValidationResult("Only PDF files are allowed.");

            return ValidationResult.Success;
        }
    }
}

using MyMvcProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.Purchases
{
    public class ValidateRequiredDate:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Cast validationContext to PurchaseViewModel
            var model = (PurchaseViewModel)validationContext.ObjectInstance;

            // Check if the value is null
            if (value != null)
            {
                DateTime requiredDate = (DateTime)value;

                // Check if the requiredDate is greater than or equal to purchase date
                if (requiredDate >= model.PurchaseDate)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Required date must be equal or greater than purchase date!");
                }
            }
            else
            {
                return new ValidationResult("Please provide required date!");
            }
        }
    }
}

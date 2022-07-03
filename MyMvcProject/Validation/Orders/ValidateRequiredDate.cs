using MyMvcProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.Orders
{
    public class ValidateRequiredDate:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Cast validationContext to OrderViewModel
            var model = (OrderViewModel)validationContext.ObjectInstance;

            // Check if the value is null
            if (value != null)
            {
                DateTime requiredDate = (DateTime)value;

                // Check if the requiredDate is greater than or equal to order date
                if (requiredDate >= model.OrderDate)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Required date must be equal or greater than order date!");
                }
            }
            else
            {
                return new ValidationResult("Please provide required date!");
            }
        }
    }
}

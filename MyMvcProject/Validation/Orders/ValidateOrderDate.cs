using MyMvcProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.Orders
{
    public class ValidateOrderDate:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Cast validationContext to OrderViewModel
            var model = (OrderViewModel)validationContext.ObjectInstance;

            // Check if the value is null
            if (value != null)
            {
                DateTime orderDate = (DateTime)value;

                // Check if the orderDate is greater than today's date
                if (orderDate.Date > DateTime.Now.Date)
                {
                    return new ValidationResult("Order date can't be greater than today's date!");
                }
                else
                {
                    // Check if the orderDate is greater than RequiredDate
                    if (orderDate > model.RequiredDate)
                    {
                        return new ValidationResult("Order date can't be greater than required date!");
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            else
            {
                return new ValidationResult("Please provide order date!");
            }
        }
    }
}

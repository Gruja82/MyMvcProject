using MyMvcProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.Productions
{
    public class ValidateProductionDate:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Cast validationContext to ProductionViewModel
            var model = (ProductionViewModel)validationContext.ObjectInstance;

            // Check if the value is null
            if (value != null)
            {
                DateTime productionDate = (DateTime)value;

                // Check if the productionDate is greater than today's date
                if (productionDate.Date > DateTime.Now.Date)
                {
                    return new ValidationResult("Production date can't be greater than today's date!");
                }
                else
                {
                    return ValidationResult.Success;

                }
            }
            else
            {
                return new ValidationResult("Please provide Production date!");
            }
        }
    }
}

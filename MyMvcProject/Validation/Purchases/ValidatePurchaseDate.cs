using MyMvcProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.Purchases
{
    public class ValidatePurchaseDate:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Cast validationContext to PurchaseViewModel
            var model = (PurchaseViewModel)validationContext.ObjectInstance;

            // Check if the value is null
            if (value != null)
            {
                DateTime purchaseDate = (DateTime)value;

                // Check if the orderDate is greater than today's date
                if (purchaseDate.Date > DateTime.Now.Date)
                {
                    return new ValidationResult("Purchase date can't be greater than today's date!");
                }
                else
                {
                    // Check if the purchaseDate is greater than RequiredDate
                    if (purchaseDate > model.RequiredDate)
                    {
                        return new ValidationResult("Purchase date can't be greater than required date!");
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            else
            {
                return new ValidationResult("Please provide purchase date!");
            }
        }
    }
}

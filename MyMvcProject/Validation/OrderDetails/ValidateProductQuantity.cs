using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.OrderDetails
{
    public class ValidateProductQuantity:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Check if the value is not null
            if (value != null)
            {
                // Declare int variable for parsing string to int
                int quantity;

                // Check is the provided value convertible to int
                if (int.TryParse(value.ToString(), out quantity))
                {
                    // Check is the value greater or equal to 0
                    if (quantity > 0)
                    {
                        return ValidationResult.Success;
                    }
                    // Otherwise return error message
                    else
                    {
                        return new ValidationResult("Quantity cannot be negative!");
                    }
                }
                // Otherwise return error message
                else
                {
                    return new ValidationResult("Product's quantity must be integer number greater than or equal to 0!");
                }
            }
            // Otherwise return error message
            else
            {
                return new ValidationResult("Please provide quantity!");
            }
        }
    }
}

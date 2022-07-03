using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.Products
{
    public class ValidateProductPrice:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Variable that is used for parsing value to float
            float parseValue;

            // If value is not null or empty string then try to convert value to float
            if (value != null && value.ToString() != String.Empty)
            {
                // Check if the value is convertible to float
                if (float.TryParse(value.ToString(), out parseValue))
                {
                    // Convert value to float
                    float floatValue = float.Parse(value.ToString());

                    // If floatValue is greater than or equal to 0 then return success
                    if (floatValue > 0)
                    {
                        return ValidationResult.Success;
                    }
                    // Otherwise return error message
                    else
                    {
                        return new ValidationResult("Price must be greater than or equal to 0!");
                    }
                }
                // Otherwise return error message
                else
                {
                    return new ValidationResult("Price must be valid decimal value!");
                }
            }
            // Otherwise return error message
            else
            {
                return new ValidationResult("Price is required!");
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.Orders
{
    public class ValidateOrderCode:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Declare and instantiate new DatabaseContext
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            using var context = new DatabaseContext(optionsBuilder.Options);

            // Cast validationContext to OrderViewModel
            var model = (OrderViewModel)validationContext.ObjectInstance;

            // Fetch all Order codes from database
            List<string> codeList = context.Orders.Select(e => e.Code).ToList();

            // Check if the value is not null or empty string
            if (value != null && value.ToString() != String.Empty)
            {
                // If model's Id is greater than 0 it means that OrderViewModel is used in Edit operation
                if (model.Id > 0)
                {
                    // Find Order record by model's Id
                    Order orderItem = context.Orders.Find(model.Id);

                    // If model's code is modified
                    if (model.Code != orderItem.Code)
                    {
                        // If supplied code is already contained in any of Order records then return error message
                        if (codeList.Contains(value.ToString()))
                        {
                            return new ValidationResult("There is already Order with this code in database. Please provide different one!");
                        }
                        // Otherwise return success
                        else
                        {
                            return ValidationResult.Success;
                        }
                    }
                    // Otherwise return success
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                // Otherwise OrderViewModel is used in Create operation
                else
                {
                    // If supplied code is already contained in any of Order records then return error message
                    if (codeList.Contains(value.ToString()))
                    {
                        return new ValidationResult("There is already Order with this code in database. Please provide different one!");
                    }
                    // Otherwise return success
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            // Otherwise return error message
            else
            {
                return new ValidationResult("Please provide Order code!");
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.Products
{
    public class ValidateProductName:ValidationAttribute
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

            // Cast validationContext to ProductViewModel
            var model = (ProductViewModel)validationContext.ObjectInstance;

            // Fetch all Product names from database
            List<string> nameList = context.Products.Select(e => e.Name).ToList();

            // Check if the value is not null or empty string
            if (value != null && value.ToString() != String.Empty)
            {
                // If model's Id is greater than 0 it means that ProductViewModel is used in Edit operation
                if (model.Id > 0)
                {
                    // Find Product record by model's Id
                    Product productItem = context.Products.Find(model.Id);

                    // If model's name is modified
                    if (model.Name != productItem.Name)
                    {
                        // If supplied name is already contained in any of Product records then return error message
                        if (nameList.Contains(value.ToString()))
                        {
                            return new ValidationResult("There is already Product with this name in database. Please provide different one!");
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
                // Otherwise ProductViewModel is used in Create operation
                else
                {
                    // If supplied code is already contained in any of Product records then return error message
                    if (nameList.Contains(value.ToString()))
                    {
                        return new ValidationResult("There is already Product with this name in database. Please provide different one!");
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
                return new ValidationResult("Please provide Product name!");
            }
        }
    }
}

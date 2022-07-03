using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.Categories
{
    public class ValidateCategoryName:ValidationAttribute
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

            // Cast validationContext to CategoryViewModel
            var model = (CategoryViewModel)validationContext.ObjectInstance;

            // Fetch all Category names from database
            List<string> namesList = context.Categories.Select(e => e.Name).ToList();

            // Check if the value is not null or empty string
            if (value != null && value.ToString() != string.Empty)
            {
                // If model's Id is greater than 0 it means that CategoryViewModel is used in Edit operation
                if (model.Id > 0)
                {
                    // Find Category record by model's Id
                    Category categoriesItem = context.Categories.Find(model.Id);

                    // If model's name is modified
                    if (model.Name != categoriesItem.Name)
                    {
                        // If supplied name is already assigned with another category in database then return error message
                        if (namesList.Contains(value.ToString()))
                        {
                            return new ValidationResult("There is already Category with this name in the database. Please provide different one!");
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
                // Otherwise CategoriesViewModel is used in Create operation
                else
                {
                    // If supplied name is already assigned with another category in database then return error message
                    if (namesList.Contains(value.ToString()))
                    {
                        return new ValidationResult("There is already Category with this name in the database. Please provide different one!");
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
                return new ValidationResult("Please provide Category name!");
            }
        }
    }
}

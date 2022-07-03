using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.Materials
{
    public class ValidateMaterialCode:ValidationAttribute
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

            // Cast validationContext to MaterialViewModel
            var model = (MaterialViewModel)validationContext.ObjectInstance;

            // Fetch all Material codes from database
            List<string> codeList = context.Materials.Select(e => e.Code).ToList();

            // Check if the value is not null or empty string
            if (value != null && value.ToString() != String.Empty)
            {
                // If model's Id is greater than 0 it means that MaterialViewModel is used in Edit operation
                if (model.Id > 0)
                {
                    // Find Material record by model's Id
                    Material materialItem = context.Materials.Find(model.Id);

                    // If model's code is modified
                    if (model.Code != materialItem.Code)
                    {
                        // If supplied code is already contained in any of Material records then return error message
                        if (codeList.Contains(value.ToString()))
                        {
                            return new ValidationResult("There is already Material with this code in database. Please provide different one!");
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
                // Otherwise MaterialViewModel is used in Create operation
                else
                {
                    // If supplied code is already contained in any of Material records then return error message
                    if (codeList.Contains(value.ToString()))
                    {
                        return new ValidationResult("There is already Material with this code in database. Please provide different one!");
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
                return new ValidationResult("Please provide Material code!");
            }
        }
    }
}

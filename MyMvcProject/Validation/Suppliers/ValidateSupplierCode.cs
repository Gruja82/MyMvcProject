using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Validation.Suppliers
{
    public class ValidateSupplierCode:ValidationAttribute
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

            // Cast validationContext to SupplierViewModel
            var model = (SupplierViewModel)validationContext.ObjectInstance;

            // Fetch all Supplier codes from database
            List<string> codeList = context.Suppliers.Select(e => e.Code).ToList();

            // Check if the value is not null or empty string
            if (value != null && value.ToString() != String.Empty)
            {
                // If model's Id is greater than 0 it means that SupplierViewModel is used in Edit operation
                if (model.Id > 0)
                {
                    // Find Supplier record by model's Id
                    Supplier supplierItem = context.Suppliers.Find(model.Id);

                    // If model's code is modified
                    if (model.Code != supplierItem.Code)
                    {
                        // If supplied code is already contained in any of Supplier records then return error message
                        if (codeList.Contains(value.ToString()))
                        {
                            return new ValidationResult("There is already Supplier with this code in database. Please provide different one!");
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
                // Otherwise SupplierViewModel is used in Create operation
                else
                {
                    // If supplied code is already contained in any of Supplier records then return error message
                    if (codeList.Contains(value.ToString()))
                    {
                        return new ValidationResult("There is already Supplier with this code in database. Please provide different one!");
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
                return new ValidationResult("Please provide Supplier code!");
            }
        }
    }
}

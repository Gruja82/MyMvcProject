using MyMvcProject.Validation.Suppliers;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Models
{
    public class SupplierViewModel
    {
        public int Id { get; set; }
        [ValidateSupplierCode]
        public string Code { get; set; }
        [ValidateSupplierCompany]
        public string Company { get; set; }
        [Required]
        public string Contact { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Postal { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        [EmailAddress]
        [ValidateSupplierEmail]
        public string Email { get; set; }
        public string? LogoPath { get; set; }
        public IFormFile? Logo { get; set; }
    }
}

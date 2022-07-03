using MyMvcProject.Validation.Customers;
using System.ComponentModel.DataAnnotations;

namespace MyMvcProject.Models
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        [ValidateCustomerCode]
        public string Code { get; set; }
        [ValidateCustomerCompany]
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
        [ValidateCustomerEmail]
        public string Email { get; set; }
        public string? LogoPath { get; set; }
        public IFormFile? Logo { get; set; }
    }
}

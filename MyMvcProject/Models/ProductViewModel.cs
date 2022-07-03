using MyMvcProject.Validation.Products;
using MyMvcProject.Validation.OrderDetails;

namespace MyMvcProject.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [ValidateProductCode]
        public string Code { get; set; }
        [ValidateProductName]
        public string Name { get; set; }
        public string? Category { get; set; }
        //[ValidateProductQuantity]
        public int InStock { get; set; }
        [ValidateProductPrice]
        public float Price { get; set; }
        public string? ImagePath { get; set; }
        public IFormFile? Image { get; set; }
        public List<ProductDetailsViewModel>? ProductDetailsList { get; set; }
    }
}

using MyMvcProject.Validation.ProductDetails;

namespace MyMvcProject.Models
{
    public class ProductDetailsViewModel
    {
        public int? Id { get; set; }
        public string? Product { get; set; }
        public string? Material { get; set; }
        [ValidateMaterialQuantity]
        public float Quantity { get; set; } = 0;
    }
}

using MyMvcProject.Validation.OrderDetails;

namespace MyMvcProject.Models
{
    public class OrderDetailViewModel
    {
        public int? Id { get; set; }
        public string? Order { get; set; }
        public string? Product { get; set; }
        [ValidateProductQuantity]
        public int Quantity { get; set; } = 0;
    }
}

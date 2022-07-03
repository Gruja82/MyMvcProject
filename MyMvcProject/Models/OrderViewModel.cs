using MyMvcProject.Validation.Orders;

namespace MyMvcProject.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        [ValidateOrderCode]
        public string Code { get; set; }
        [ValidateOrderDate]
        public DateTime OrderDate { get; set; }
        [ValidateRequiredDate]
        public DateTime RequiredDate { get; set; }
        public string? Customer { get; set; }
        public bool Completed { get; set; }
        public List<OrderDetailViewModel>? OrderDetailList { get; set; }
    }
}

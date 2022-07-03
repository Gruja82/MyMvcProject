using MyMvcProject.Validation.PurchaseDetails;

namespace MyMvcProject.Models
{
    public class PurchaseDetailViewModel
    {
        public int? Id { get; set; }
        public string? Purchase { get; set; }
        public string? Material { get; set; }
        [ValidateMaterialQuantity]
        public float Quantity { get; set; }
    }
}

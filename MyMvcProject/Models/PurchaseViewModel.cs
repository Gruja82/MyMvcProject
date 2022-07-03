using MyMvcProject.Validation.Purchases;

namespace MyMvcProject.Models
{
    public class PurchaseViewModel
    {
        public int Id { get; set; }
        [ValidatePurchaseCode]
        public string Code { get; set; }
        [ValidatePurchaseDate]
        public DateTime PurchaseDate { get; set; }
        [ValidateRequiredDate]
        public DateTime RequiredDate { get; set; }
        public bool Completed { get; set; }
        public string? Supplier { get; set; }
        public List<PurchaseDetailViewModel>? PurchaseDetailList { get; set; }
    }
}

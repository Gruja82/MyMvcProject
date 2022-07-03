using MyMvcProject.Validation.Productions;

namespace MyMvcProject.Models
{
    public class ProductionViewModel
    {
        public int Id { get; set; }
        [ValidateProductionCode]
        public string Code { get; set; }
        [ValidateProductionDate]
        public DateTime ProductionDate { get; set; }
        public List<ProductionDetailViewModel>? ProductionDetailsList { get; set; }
    }
}

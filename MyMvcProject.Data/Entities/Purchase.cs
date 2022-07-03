using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Entities
{
    public class Purchase:BaseEntity
    {
        public string Code { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public int? SupplierId { get; set; }
        public bool Completed { get; set; }
        public Supplier Supplier { get; set; }
        public List<PurchaseDetail> PurchaseDetails { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Entities
{
    public class Material:BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public float InStock { get; set; }
        public float Price { get; set; }
        public string? ImagePath { get; set; }
        public Category Category { get; set; }
        public List<PurchaseDetail> PurchaseDetails { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
    }
}

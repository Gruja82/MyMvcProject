using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Entities
{
    public class Product:BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public int InStock { get; set; }
        public float Price { get; set; }
        public string? ImagePath { get; set; }
        public Category Category { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
        public List<ProductionDetail> ProductionDetails { get; set; }
    }
}

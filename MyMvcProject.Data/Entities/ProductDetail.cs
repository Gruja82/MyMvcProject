using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Entities
{
    public class ProductDetail:BaseEntity
    {
        public int? ProductId { get; set; }
        public int? MaterialId { get; set; }
        public float Quantity { get; set; }
        public Product Product { get; set; }
        public Material Material { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Entities
{
    public class PurchaseDetail:BaseEntity
    {
        public int? PurchaseId { get; set; }
        public int? MaterialId { get; set; }
        public float Quantity { get; set; }
        public Purchase Purchase { get; set; }
        public Material Material { get; set; }
    }
}

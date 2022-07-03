using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Entities
{
    public class ProductionDetail:BaseEntity
    {
        public int? ProductionId { get; set; }
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public Production Production { get; set; }
        public Product Product { get; set; }

    }
}

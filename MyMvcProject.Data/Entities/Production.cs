using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Entities
{
    public class Production:BaseEntity
    {
        public string Code { get; set; }
        public DateTime ProductionDate { get; set; }
        public List<ProductionDetail> ProductionDetails { get; set; }
    }
}

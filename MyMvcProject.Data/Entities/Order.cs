using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Entities
{
    public class Order:BaseEntity
    {
        public string Code { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public int? CustomerId { get; set; }
        public bool Completed { get; set; }
        public Customer Customer { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}

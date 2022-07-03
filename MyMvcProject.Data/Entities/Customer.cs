using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Entities
{
    public class Customer:BaseEntity
    {
        public string Code { get; set; }
        public string Company { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Postal { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? LogoPath { get; set; }
        public List<Order> Orders { get; set; }
    }
}

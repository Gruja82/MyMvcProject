using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Entities
{
    public class Category:BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public List<Product> Products { get; set; }
        public List<Material> Materials { get; set; }
    }
}

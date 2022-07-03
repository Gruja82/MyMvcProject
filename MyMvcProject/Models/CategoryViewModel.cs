using MyMvcProject.Validation.Categories;

namespace MyMvcProject.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        [ValidateCategoryName]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public IFormFile? Image { get; set; }
    }
}

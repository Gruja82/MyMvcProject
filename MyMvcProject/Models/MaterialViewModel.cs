using MyMvcProject.Validation.Materials;

namespace MyMvcProject.Models
{
    public class MaterialViewModel
    {
        public int Id { get; set; }
        [ValidateMaterialCode]
        public string Code { get; set; }
        [ValidateMaterialName]
        public string Name { get; set; }
        [ValidateMaterialFloat]
        public float InStock { get; set; }
        [ValidateMaterialFloat]
        public float Price { get; set; }
        public string? Category { get; set; }
        public string? ImagePath { get; set; }
        public IFormFile? Image { get; set; }
    }
}

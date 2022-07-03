using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;

namespace MyMvcProject.Controllers
{
    public class MaterialController : Controller
    {
        private readonly IDbContextFactory<DatabaseContext> factory;
        private readonly IWebHostEnvironment webHostEnvironment;
        public MaterialController(IDbContextFactory<DatabaseContext> factory, IWebHostEnvironment webHostEnvironment)
        {
            this.factory = factory;
            this.webHostEnvironment = webHostEnvironment;
        }
        // Helper method for implementing Paging
        private static Pagination<MaterialViewModel> GetPagination(int currentPage, List<MaterialViewModel> viewModelList)
        {
            // Max items shown per page
            int maxRows = 2;

            // Declare and instantiate new Pagination<T>
            Pagination<MaterialViewModel> pagination = new();

            // Set DataList
            pagination.DataList = (from c in viewModelList select c)
                .Skip((currentPage - 1) * maxRows)
                .Take(maxRows)
                .ToList();

            // Calculate number of pages
            double pageCount = (double)((decimal)viewModelList.Count / Convert.ToDecimal(maxRows));

            // Set PageCount property usin Math.Ceiling which return smallest integral value greater than or equal to specified number
            pagination.PageCount = (int)Math.Ceiling(pageCount);

            // Set CurrentPageIndex property to provided currentPage argument value
            pagination.CurrentPageIndex = currentPage;

            // Return object
            return pagination;
        }

        // Helper method for storing images to server and returning file path
        private string SaveImage(MaterialViewModel model)
        {
            string imageFilename = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/Materials");
                imageFilename = Guid.NewGuid().ToString().Substring(0, 10) + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, imageFilename);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                model.Image.CopyTo(fileStream);
            }

            return imageFilename;
        }

        // Shared method for Details,Edit and Delete Get methods
        private async Task<IActionResult> GetMethod(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Material record by id
            Material materialItem = await context.Materials.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new MaterialViewModel
            MaterialViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = materialItem.Id;
            viewModelItem.Code = materialItem.Code;
            viewModelItem.Name = materialItem.Name;
            viewModelItem.Category = materialItem.Category.Name;
            viewModelItem.InStock = materialItem.InStock;
            viewModelItem.Price = materialItem.Price;
            viewModelItem.ImagePath = materialItem.ImagePath;

            ViewData["Categories"] = await context.Categories.Select(e => e.Name).ToListAsync();

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // GET - Action method for returning list of all Material records
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Declare and instantiate new List<MaterialsViewModel>
            List<MaterialViewModel> viewModelList = new();

            // Iterate through all Material records and populate viewModelList
            foreach (var material in context.Materials.Include(e => e.Category).AsNoTracking())
            {
                MaterialViewModel viewModelItem = new();

                viewModelItem.Id = material.Id;
                viewModelItem.Code = material.Code;
                viewModelItem.Name = material.Name;
                viewModelItem.Category = material.Category.Name;
                viewModelItem.InStock = material.InStock;
                viewModelItem.Price = material.Price;
                viewModelItem.ImagePath = material.ImagePath;

                viewModelList.Add(viewModelItem);
            }

            // Store Category names in ViewData object
            ViewData["Categories"] = await context.Categories.Select(e => e.Name).ToListAsync();

            // Return View with Pagination
            return View(GetPagination(1, viewModelList));
        }

        // POST - Action method which is called when Search or Page number button is clicked
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index(string searchText, string category, int pageIndex = 1)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Load all Material records from database
            List<Material> materialList = await context.Materials.Include(e => e.Category).AsNoTracking().ToListAsync();

            // If searchText is not null or empty string then filter the materialList by searchText
            if (searchText != null && searchText != String.Empty)
            {
                materialList = materialList.Where(
                    e => e.Code.ToLower().Contains(searchText.ToLower())
                    || e.Name.ToLower().Contains(searchText.ToLower()))
                    .ToList();
            }

            // If category is not equal to it's default value
            if (category != "Select category")
            {
                materialList = materialList.Where(e => e.Category == context.Categories.FirstOrDefault(e => e.Name == category)).ToList();
            }

            // Declare and instantiate new List<MaterialsViewModel>
            List<MaterialViewModel> viewModelList = new();

            // Iterate through all Material records and populate viewModelList
            foreach (var material in materialList)
            {
                MaterialViewModel viewModelItem = new();

                viewModelItem.Id = material.Id;
                viewModelItem.Code = material.Code;
                viewModelItem.Name = material.Name;
                viewModelItem.Category = material.Category.Name;
                viewModelItem.InStock = material.InStock;
                viewModelItem.Price = material.Price;
                viewModelItem.ImagePath = material.ImagePath;

                viewModelList.Add(viewModelItem);
            }

            // Store Category names in ViewData object
            ViewData["Categories"] = await context.Categories.Select(e => e.Name).ToListAsync();

            // Return View with Pagination
            return View(GetPagination(pageIndex, viewModelList));
        }

        // GET - Action method for displaying page with full details for selected Material
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Details(int id)
        {
            return await GetMethod(id);
        }

        // GET - Action method for displaying page for creating new Material
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Store Category names in ViewData object
            ViewData["Categories"] = await context.Categories.Select(e => e.Name).ToListAsync();

            return View();
        }

        // POST - Action method for creating new Material
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaterialViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If model's values are valid
            if (ModelState.IsValid)
            {
                // Declare and instantiate new Material
                Material materialItem = new();

                // Set properties
                materialItem.Code = model.Code;
                materialItem.Name = model.Name;
                materialItem.Category = await context.Categories.FirstOrDefaultAsync(e => e.Name == model.Category);
                materialItem.InStock = 0;
                materialItem.Price = model.Price;
                materialItem.ImagePath = SaveImage(model);

                // Add new Material record and save changes to database
                await context.Materials.AddAsync(materialItem);
                await context.SaveChangesAsync();

                // Redirect to Details page
                return RedirectToAction(nameof(Details), new { id = materialItem.Id });
            }
            // Otherwise return same page with errors
            else
            {
                return View(model);
            }
        }

        // GET - Action method for displaying page for editing selected Material
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            return await GetMethod(id);
        }

        // POST - Action method for editing selected Material
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MaterialViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If model's values are valid
            if (ModelState.IsValid)
            {
                // Find Material by model's Id
                Material materialItem = await context.Materials.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == model.Id);

                // Set properties
                materialItem.Code = model.Code;
                materialItem.Name = model.Name;
                materialItem.Category = await context.Categories.FirstOrDefaultAsync(e => e.Name == model.Category);
                materialItem.Price = model.Price;
                if (model.Image != null)
                {
                    if (System.IO.File.Exists(Path.Combine(webHostEnvironment.WebRootPath, "images/Materials/" + materialItem.ImagePath)))
                    {
                        System.IO.File.Delete(Path.Combine(webHostEnvironment.WebRootPath, "images/Materials/" + materialItem.ImagePath));
                    }

                    materialItem.ImagePath = SaveImage(model);
                }

                // Save changes to database
                await context.SaveChangesAsync();

                // Redirect to Details page
                return RedirectToAction(nameof(Details), new { id = materialItem.Id });
            }
            // Otherwise return same page with errors
            else
            {
                return View(model);
            }
        }

        // GET - Action method for displaying page for deleting selected Material
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await GetMethod(id);
        }

        // POST - Action method for deleting selected Material
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(MaterialViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Material by model's Id
            Material materialItem = await context.Materials.FindAsync(model.Id);

            // Delete selected Material and save changes to database
            context.Materials.Remove(materialItem);
            await context.SaveChangesAsync();

            // Redirect to Index page
            return RedirectToAction(nameof(Index));
        }
    }
}

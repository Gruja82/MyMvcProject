using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;

namespace MyMvcProject.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IDbContextFactory<DatabaseContext> factory;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CategoryController(IDbContextFactory<DatabaseContext> factory, IWebHostEnvironment webHostEnvironment)
        {
            this.factory = factory;
            this.webHostEnvironment = webHostEnvironment;
        }

        // Helper method for implementing Paging
        private static Pagination<CategoryViewModel> GetPagination(int currentPage, List<CategoryViewModel> viewModelList)
        {
            // Max items shown per page
            int maxRows = 2;

            // Declare and instantiate new Pagination<T>
            Pagination<CategoryViewModel> pagination = new();

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
        private string SaveImage(CategoryViewModel model)
        {
            string imageFilename = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/Categories");
                imageFilename = Guid.NewGuid().ToString().Substring(0, 10) + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, imageFilename);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                model.Image.CopyTo(fileStream);
            }

            return imageFilename;
        }

        // GET - Action method for returning list of all Categories
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Declare and instantiate new List<CategoryViewModel>
            List<CategoryViewModel> viewModelList = new();

            // Iterate through all Category records and populate viewModelList
            foreach (var category in context.Categories)
            {
                // Declare and instantiate new CategoryViewModel
                CategoryViewModel viewModelItem = new();

                // Set properties
                viewModelItem.Id = category.Id;
                viewModelItem.Name = category.Name;
                viewModelItem.Description = category.Description;
                viewModelItem.ImagePath = category.ImagePath;

                // Add viewModelItem to viewModelList
                viewModelList.Add(viewModelItem);
            }

            // Return View with Pagination<CategoryViewModel>
            return View(GetPagination(1, viewModelList));
        }

        // POST - Action method which is called when Search or Page number button is clicked
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index(string searchText, int pageIndex = 1)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Load all Category records from database
            List<Category> categoriesList = await context.Categories.ToListAsync();

            // If searchText is not null or empty string then filter the categoriesList by searchText
            if (searchText != null && searchText != String.Empty)
            {
                categoriesList = categoriesList.Where(e => e.Name.ToLower().Contains(searchText.ToLower())).ToList();
            }

            // Declare and instantiate new List<CategoryViewModel>
            List<CategoryViewModel> viewModelList = new();

            // Iterate through all Category records and populate viewModelList
            foreach (var category in categoriesList)
            {
                // Declare and instantiate new CategoryViewModel
                CategoryViewModel viewModelItem = new();

                // Set properties
                viewModelItem.Id = category.Id;
                viewModelItem.Name = category.Name;
                viewModelItem.Description = category.Description;
                viewModelItem.ImagePath = category.ImagePath;

                // Add viewModelItem to viewModelList
                viewModelList.Add(viewModelItem);
            }

            // Return View with Pagination<CategoryViewModel>
            return View(GetPagination(pageIndex, viewModelList));
        }

        // GET - Action method for displaying page with full details for selected Category
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Details(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Category record by id
            Category categoryItem = await context.Categories.FindAsync(id);

            // Declare and instantiate new CategoryViewModel
            CategoryViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = categoryItem.Id;
            viewModelItem.Name = categoryItem.Name;
            viewModelItem.Description = categoryItem.Description;
            viewModelItem.ImagePath = categoryItem.ImagePath;

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // GET - Action method for displaying page for creating new Category
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST - Action method for creating new Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            if (ModelState.IsValid)
            {
                // Declare and instantiate new Category
                Category categoryItem = new();

                // Set properties
                categoryItem.Name = model.Name;
                categoryItem.Description = model.Description;
                categoryItem.ImagePath = SaveImage(model);

                // Add new Category record to database and save changes
                await context.AddAsync(categoryItem);
                await context.SaveChangesAsync();

                // Redirect to Details page
                return RedirectToAction(nameof(Details), new { id = categoryItem.Id });
            }
            else
            {
                return View(model);
            }
        }

        // GET - Action method for displaying page for editing selected Category
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Category record by id
            Category categoryItem = await context.Categories.FindAsync(id);

            // Declare and instantiate new CategoryViewModel
            CategoryViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = categoryItem.Id;
            viewModelItem.Name = categoryItem.Name;
            viewModelItem.Description = categoryItem.Description;
            viewModelItem.ImagePath = categoryItem.ImagePath;

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // POST - Action method for editing selected Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            if (ModelState.IsValid)
            {
                // Find Category record by model's Id
                Category categoryItem = await context.Categories.FindAsync(model.Id);

                // Set properties
                categoryItem.Name = model.Name;
                categoryItem.Description = model.Description;
                if (model.Image != null)
                {
                    if (System.IO.File.Exists(Path.Combine(webHostEnvironment.WebRootPath, "images/Categories/" + categoryItem.ImagePath)))
                    {
                        System.IO.File.Delete(Path.Combine(webHostEnvironment.WebRootPath, "images/Categories/" + categoryItem.ImagePath));
                    }

                    categoryItem.ImagePath = SaveImage(model);
                }

                // Save changes to database
                await context.SaveChangesAsync();

                // Redirect to Details page
                return RedirectToAction(nameof(Details), new { id = categoryItem.Id });
            }
            else
            {
                return View(model);
            }
        }

        // GET - Action method for displaying page with selected Category to delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Category record by id
            Category categoryItem = await context.Categories.FindAsync(id);

            // Declare and instantiate new CategoryViewModel
            CategoryViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = categoryItem.Id;
            viewModelItem.Name = categoryItem.Name;
            viewModelItem.Description = categoryItem.Description;
            viewModelItem.ImagePath = categoryItem.ImagePath;

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // POST - Action method for deleting selected Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(CategoryViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Category record by model's Id
            Category categoryItem = await context.Categories.FindAsync(model.Id);

            // Delete selected Category and save changes to database
            context.Categories.Remove(categoryItem);
            await context.SaveChangesAsync();

            // Redirect to Index page
            return RedirectToAction(nameof(Index));
        }
    }
}

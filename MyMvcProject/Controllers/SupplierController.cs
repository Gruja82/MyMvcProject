using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;

namespace MyMvcProject.Controllers
{
    public class SupplierController : Controller
    {
        private readonly IDbContextFactory<DatabaseContext> factory;
        private readonly IWebHostEnvironment webHostEnvironment;
        public SupplierController(IDbContextFactory<DatabaseContext> factory, IWebHostEnvironment webHostEnvironment)
        {
            this.factory = factory;
            this.webHostEnvironment = webHostEnvironment;
        }

        // Helper method for implementing Paging
        private static Pagination<SupplierViewModel> GetPagination(int currentPage, List<SupplierViewModel> viewModelList)
        {
            // Max items shown per page
            int maxRows = 2;

            // Declare and instantiate new Pagination<T>
            Pagination<SupplierViewModel> pagination = new();

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
        private string SaveImage(SupplierViewModel model)
        {
            string imageFilename = null;

            if (model.Logo != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/Suppliers");
                imageFilename = Guid.NewGuid().ToString().Substring(0, 10) + "_" + model.Logo.FileName;
                string filePath = Path.Combine(uploadsFolder, imageFilename);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                model.Logo.CopyTo(fileStream);
            }

            return imageFilename;
        }

        // Shared method for Details,Edit and Delete Get methods
        private async Task<IActionResult> GetMethod(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Supplier record by Id
            Supplier supplierItem = await context.Suppliers.FindAsync(id);

            // Declare and instantiate new SupplierViewModel
            SupplierViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = supplierItem.Id;
            viewModelItem.Code = supplierItem.Code;
            viewModelItem.Company = supplierItem.Company;
            viewModelItem.Contact = supplierItem.Contact;
            viewModelItem.Address = supplierItem.Address;
            viewModelItem.City = supplierItem.City;
            viewModelItem.Postal = supplierItem.Postal;
            viewModelItem.Phone = supplierItem.Phone;
            viewModelItem.Email = supplierItem.Email;
            viewModelItem.LogoPath = supplierItem.LogoPath;

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // GET - Action method for returning list of all Suppliers
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Declare and instantiate new List<SupplierViewModel>
            List<SupplierViewModel> viewModelList = new();

            // Iterate through all Supplier records and populate viewModelList
            foreach (var customer in context.Customers)
            {
                // Declare and instantiate new SupplierViewModel
                SupplierViewModel viewModelItem = new();

                // Set properties
                viewModelItem.Id = customer.Id;
                viewModelItem.Code = customer.Code;
                viewModelItem.Company = customer.Company;
                viewModelItem.Contact = customer.Contact;
                viewModelItem.Address = customer.Address;
                viewModelItem.City = customer.City;
                viewModelItem.Postal = customer.Postal;
                viewModelItem.Phone = customer.Phone;
                viewModelItem.Email = customer.Email;
                viewModelItem.LogoPath = customer.LogoPath;

                viewModelList.Add(viewModelItem);
            }

            // Return View with Pagination
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

            // Load all Supplier records from database
            List<Supplier> supplierList = await context.Suppliers.ToListAsync();

            // If searchText is not null or empty string then filter the supplierList by searchText
            if (searchText != null && searchText != String.Empty)
            {
                supplierList = supplierList.Where(
                    e => e.Code.ToLower().Contains(searchText.ToLower())
                    || e.Company.ToLower().Contains(searchText.ToLower()))
                    .ToList();
            }

            // Declare and instantiate new List<SupplierViewModel>
            List<SupplierViewModel> viewModelList = new();

            // Iterate through all Supplier records and populate viewModelList
            foreach (var customer in supplierList)
            {
                // Declare and instantiate new SupplierViewModel
                SupplierViewModel viewModelItem = new();

                // Set properties
                viewModelItem.Id = customer.Id;
                viewModelItem.Code = customer.Code;
                viewModelItem.Company = customer.Company;
                viewModelItem.Contact = customer.Contact;
                viewModelItem.Address = customer.Address;
                viewModelItem.City = customer.City;
                viewModelItem.Postal = customer.Postal;
                viewModelItem.Phone = customer.Phone;
                viewModelItem.Email = customer.Email;
                viewModelItem.LogoPath = customer.LogoPath;

                viewModelList.Add(viewModelItem);
            }

            // Return View with Pagination
            return View(GetPagination(pageIndex, viewModelList));
        }

        // GET - Action method for displaying page with full details for selected Supplier
        [HttpGet]
        [ResponseCache(Duration =60)]
        public async Task<IActionResult> Details(int id)
        {
            return await GetMethod(id);
        }

        // GET - Action method for displaying page for creating new Supplier
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST - Action method for creating new Supplier
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Declare and instantiate new Supplier
            Supplier supplierItem = new();

            // If all values are valid
            if (ModelState.IsValid)
            {
                // Set properties
                supplierItem.Code = model.Code;
                supplierItem.Company = model.Company;
                supplierItem.Contact = model.Contact;
                supplierItem.Address = model.Address;
                supplierItem.City = model.City;
                supplierItem.Postal = model.Postal;
                supplierItem.Phone = model.Phone;
                supplierItem.Email = model.Email;
                supplierItem.LogoPath = SaveImage(model);

                // Add new Supplier record to database and save changes
                await context.Suppliers.AddAsync(supplierItem);
                await context.SaveChangesAsync();

                // Redirect to Details page
                return RedirectToAction(nameof(Details), new { id = supplierItem.Id });
            }
            // Otherwise return same View with errors
            else
            {
                return View(model);
            }
        }

        // GET - Action method for displaying page for editing selected Supplier
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            return await GetMethod(id);
        }

        // POST - Action method for editing selected Supplier
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If all values are valid
            if (ModelState.IsValid)
            {
                // Find Supplier record by model's Id
                Supplier supplierItem = await context.Suppliers.FindAsync(model.Id);

                // Set properties
                supplierItem.Code = model.Code;
                supplierItem.Company = model.Company;
                supplierItem.Contact = model.Contact;
                supplierItem.Address = model.Address;
                supplierItem.City = model.City;
                supplierItem.Postal = model.Postal;
                supplierItem.Phone = model.Phone;
                supplierItem.Email = model.Email;
                if (model.Logo != null)
                {
                    if (System.IO.File.Exists(Path.Combine(webHostEnvironment.WebRootPath, "images/Suppliers/" + supplierItem.LogoPath)))
                    {
                        System.IO.File.Delete(Path.Combine(webHostEnvironment.WebRootPath, "images/Suppliers/" + supplierItem.LogoPath));
                    }

                    supplierItem.LogoPath = SaveImage(model);
                }

                // Save changes to database
                await context.SaveChangesAsync();

                // Redirect to Details page
                return RedirectToAction(nameof(Details), new { id = supplierItem.Id });
            }
            // Otherwise return same View with errors
            else
            {
                return View(model);
            }
        }

        // GET - Action method for displaying page with Supplier to delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await GetMethod(id);
        }

        // POST - Action method for deleting selected Supplier
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(SupplierViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Supplier record by Id
            Supplier supplierItem = await context.Suppliers.FindAsync(model.Id);

            // Delete selected Supplier and save changes to database
            context.Suppliers.Remove(supplierItem);
            await context.SaveChangesAsync();

            // Redirect to Index page
            return RedirectToAction(nameof(Index));
        }
    }
}

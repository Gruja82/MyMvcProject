using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;

namespace MyMvcProject.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IDbContextFactory<DatabaseContext> factory;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CustomerController(IDbContextFactory<DatabaseContext> factory, IWebHostEnvironment webHostEnvironment)
        {
            this.factory = factory;
            this.webHostEnvironment = webHostEnvironment;
        }

        // Helper method for implementing Paging
        private static Pagination<CustomerViewModel> GetPagination(int currentPage, List<CustomerViewModel> viewModelList)
        {
            // Max items shown per page
            int maxRows = 2;

            // Declare and instantiate new Pagination<T>
            Pagination<CustomerViewModel> pagination = new();

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
        private string SaveImage(CustomerViewModel model)
        {
            string imageFilename = null;

            if (model.Logo != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/Customers");
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

            // Find Customer record by Id
            Customer customerItem = await context.Customers.FindAsync(id);

            // Declare and instantiate new CustomerViewModel
            CustomerViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = customerItem.Id;
            viewModelItem.Code = customerItem.Code;
            viewModelItem.Company = customerItem.Company;
            viewModelItem.Contact = customerItem.Contact;
            viewModelItem.Address = customerItem.Address;
            viewModelItem.City = customerItem.City;
            viewModelItem.Postal = customerItem.Postal;
            viewModelItem.Phone = customerItem.Phone;
            viewModelItem.Email = customerItem.Email;
            viewModelItem.LogoPath = customerItem.LogoPath;

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // GET - Action method for returning list of all Customers
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Declare and instantiate new List<CustomerViewModel>
            List<CustomerViewModel> viewModelList = new();

            // Iterate through all Customer records and populate viewModelList
            foreach (var customer in context.Customers)
            {
                // Declare and instantiate new CustomerViewModel
                CustomerViewModel viewModelItem = new();

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

            // Load all Customer records from database
            List<Customer> customersList = await context.Customers.ToListAsync();

            // If searchText is not null or empty string then filter the customersList by searchText
            if (searchText != null && searchText != String.Empty)
            {
                customersList = customersList.Where(
                    e => e.Code.ToLower().Contains(searchText.ToLower())
                    || e.Company.ToLower().Contains(searchText.ToLower()))
                    .ToList();
            }

            // Declare and instantiate new List<CustomerViewModel>
            List<CustomerViewModel> viewModelList = new();

            // Iterate through all Customer records and populate viewModelList
            foreach (var customer in customersList)
            {
                // Declare and instantiate new CustomerViewModel
                CustomerViewModel viewModelItem = new();

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

        // GET - Action method for displaying page with full details for selected Customer
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return await GetMethod(id);
        }

        // GET - Action method for displaying page for creating new Customer
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST - Action method for creating new Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Declare and instantiate new Customer
            Customer customerItem = new();

            // If all values are valid
            if (ModelState.IsValid)
            {
                // Set properties
                customerItem.Code = model.Code;
                customerItem.Company = model.Company;
                customerItem.Contact = model.Contact;
                customerItem.Address = model.Address;
                customerItem.City = model.City;
                customerItem.Postal = model.Postal;
                customerItem.Phone = model.Phone;
                customerItem.Email = model.Email;
                customerItem.LogoPath = SaveImage(model);

                // Add new Customer record to database and save changes
                await context.Customers.AddAsync(customerItem);
                await context.SaveChangesAsync();

                // Redirect to Details page
                return RedirectToAction(nameof(Details), new { id = customerItem.Id });
            }
            // Otherwise return same View with errors
            else
            {
                return View(model);
            }
        }

        // GET - Action method for displaying page for editing selected Customer
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            return await GetMethod(id);
        }

        // POST - Action method for editing selected Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If all values are valid
            if (ModelState.IsValid)
            {
                // Find Customer record by model's Id
                Customer customerItem = await context.Customers.FindAsync(model.Id);

                // Set properties
                customerItem.Code = model.Code;
                customerItem.Company = model.Company;
                customerItem.Contact = model.Contact;
                customerItem.Address = model.Address;
                customerItem.City = model.City;
                customerItem.Postal = model.Postal;
                customerItem.Phone = model.Phone;
                customerItem.Email = model.Email;
                if (model.Logo != null)
                {
                    if (System.IO.File.Exists(Path.Combine(webHostEnvironment.WebRootPath, "images/Customers/" + customerItem.LogoPath)))
                    {
                        System.IO.File.Delete(Path.Combine(webHostEnvironment.WebRootPath, "images/Customers/" + customerItem.LogoPath));
                    }

                    customerItem.LogoPath = SaveImage(model);
                }

                // Save changes to database
                await context.SaveChangesAsync();

                // Redirect to Details page
                return RedirectToAction(nameof(Details), new { id = customerItem.Id });
            }
            // Otherwise return same View with errors
            else
            {
                return View(model);
            }
        }

        // GET - Action method for displaying page with Customer to delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await GetMethod(id);
        }

        // POST - Action method for deleting selected Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(CustomerViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Customer record by Id
            Customer customerItem = await context.Customers.FindAsync(model.Id);

            // Delete selected Customer and save changes to database
            context.Customers.Remove(customerItem);
            await context.SaveChangesAsync();

            // Redirect to Index page
            return RedirectToAction(nameof(Index));
        }
    }
}

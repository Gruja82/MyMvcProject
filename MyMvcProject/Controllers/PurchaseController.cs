using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;

namespace MyMvcProject.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IDbContextFactory<DatabaseContext> factory;
        public PurchaseController(IDbContextFactory<DatabaseContext> factory)
        {
            this.factory = factory;
        }
        // Helper method for implementing Paging
        private static Pagination<PurchaseViewModel> GetPagination(int currentPage, List<PurchaseViewModel> viewModelList)
        {
            // Max items shown per page
            int maxRows = 2;

            // Declare and instantiate new Pagination<T>
            Pagination<PurchaseViewModel> pagination = new();

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

        // GET - Action method for returning all Purchase records
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Declare and instantiate new List<PurchaseViewModel>
            List<PurchaseViewModel> viewModelList = new List<PurchaseViewModel>();

            // Iterate through all Order records and populate viewModelList
            foreach (var purchase in context.Purchases.Include(e => e.Supplier).AsNoTracking())
            {
                // Declare and instantiate new PurchaseViewModel
                PurchaseViewModel viewModelItem = new();

                // Set properties
                viewModelItem.Id = purchase.Id;
                viewModelItem.Code = purchase.Code;
                viewModelItem.PurchaseDate = purchase.PurchaseDate;
                viewModelItem.RequiredDate = purchase.RequiredDate;
                viewModelItem.Completed = purchase.Completed;

                viewModelList.Add(viewModelItem);
            }

            // Store Supplier company names in ViewData
            ViewData["Suppliers"] = await context.Suppliers.Select(e => e.Company).ToListAsync();

            // Return View with Pagination
            return View(GetPagination(1, viewModelList));
        }

        // POST - Action method which is called when Search or Page number button is clicked
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index(string searchText, string supplier, int pageIndex = 1)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Load all Purchase records from database
            List<Purchase> purchaseList = await context.Purchases.Include(e => e.Supplier).ToListAsync();

            // If searchText is not null or empty string then filter the purchaseList by searchText
            if (searchText != null && searchText != String.Empty)
            {
                purchaseList = purchaseList.Where(
                    e => e.Code.ToLower().Contains(searchText.ToLower()))
                    .ToList();
            }

            // If supplier is not equal to it's default value then filter the purchaseList by supplier
            if (supplier != "Select supplier")
            {
                purchaseList = purchaseList.Where(e => e.Supplier == context.Suppliers.FirstOrDefault(e => e.Company == supplier)).ToList();
            }

            // Declare and instantiate new List<PurchaseViewModel>
            List<PurchaseViewModel> viewModelList = new List<PurchaseViewModel>();

            // Iterate through all Purchase records and populate viewModelList
            foreach (var purchase in purchaseList)
            {
                // Declare and instantiate new PurchaseViewModel
                PurchaseViewModel viewModelItem = new();

                // Set properties
                viewModelItem.Id = purchase.Id;
                viewModelItem.Code = purchase.Code;
                viewModelItem.PurchaseDate = purchase.PurchaseDate;
                viewModelItem.RequiredDate = purchase.RequiredDate;
                viewModelItem.Completed = purchase.Completed;

                viewModelList.Add(viewModelItem);
            }

            // Store Supplier company names in ViewData
            ViewData["Suppliers"] = await context.Suppliers.Select(e => e.Company).ToListAsync();

            // Return View with Pagination
            return View(GetPagination(pageIndex, viewModelList));
        }

        // GET - Action method for displaying page with full details for selected Purchase
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Details(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Variable that will hold total ammount of purchase
            float total = 0;

            // Find Purchase record by id
            Purchase purchaseItem = await context.Purchases
                .Include(e => e.Supplier)
                .Include(e => e.PurchaseDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<PurchaseDetailViewModel>
            List<PurchaseDetailViewModel> detailList = new();

            // Iterate through purchaseItem.PurchaseDetails and populate detailList
            foreach (var detail in purchaseItem.PurchaseDetails)
            {
                // Declare and instantiate new PurchaseDetailViewModel
                PurchaseDetailViewModel detailViewModelItemItem = new();

                // Set properties
                detailViewModelItemItem.Id = detail.Id;
                detailViewModelItemItem.Purchase = purchaseItem.Code;
                detailViewModelItemItem.Material = detail.Material.Name;
                detailViewModelItemItem.Quantity = detail.Quantity;

                detailList.Add(detailViewModelItemItem);

                total += detail.Material.Price * detail.Quantity;
            }

            // Declare and instantiate new PurchaseViewModel
            PurchaseViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = id;
            viewModelItem.Code = purchaseItem.Code;
            viewModelItem.PurchaseDate = purchaseItem.PurchaseDate;
            viewModelItem.RequiredDate = purchaseItem.RequiredDate;
            viewModelItem.Completed = purchaseItem.Completed;
            viewModelItem.Supplier = purchaseItem.Supplier.Company;
            viewModelItem.PurchaseDetailList = detailList;

            // Store total variable in ViewData
            ViewData["Total"] = Math.Round(total, 2);

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // GET - Action method for displaying page for creating new Purchase
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Store Supplier company names and Materials in ViewData
            ViewData["Suppliers"] = await context.Suppliers.Select(e => e.Company).ToListAsync();
            ViewData["Materials"] = await context.Materials.ToListAsync();

            // Return View
            return View();
        }

        // POST - Action method for creating new Purchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If model's values are valid
            if (ModelState.IsValid)
            {
                // Declare and instantiate new Purchase
                Purchase purchaseItem = new();

                // Check if the PurchaseDetailList has any elements
                if (model.PurchaseDetailList != null && model.PurchaseDetailList.Any() && model.PurchaseDetailList.Where(e => e.Quantity > 0).Any())
                {
                    // Set properties of purchaseItem
                    purchaseItem.Code = model.Code;
                    purchaseItem.PurchaseDate = model.PurchaseDate;
                    purchaseItem.RequiredDate = model.RequiredDate;
                    purchaseItem.Completed = model.Completed;
                    purchaseItem.Supplier = await context.Suppliers.FirstOrDefaultAsync(e => e.Company == model.Supplier);

                    // Add new Purchase record and save changes to database
                    context.Purchases.Add(purchaseItem);
                    context.SaveChanges();

                    // Iterate through model.PurchaseDetailList and add PurchaseDetail records to database
                    foreach (var detail in model.PurchaseDetailList)
                    {
                        PurchaseDetail purchaseDetailItem = new();

                        purchaseDetailItem.Purchase = purchaseItem;
                        purchaseDetailItem.Material = await context.Materials.FirstOrDefaultAsync(e => e.Name == detail.Material);
                        purchaseDetailItem.Quantity = detail.Quantity;

                        context.PurchaseDetails.Add(purchaseDetailItem);

                        Material material = await context.Materials.FirstOrDefaultAsync(e => e.Name == detail.Material);

                        material.InStock += detail.Quantity;

                    }

                    // Save changes to database
                    await context.SaveChangesAsync();

                    // Redirect to Details page
                    return RedirectToAction(nameof(Details), new { id = purchaseItem.Id });
                }
                // Otherwise return same page with errors
                else
                {
                    ViewData["Error"] = "There must be at least one material in list!";
                    // Store Supplier company names and Materials in ViewData
                    ViewData["Suppliers"] = await context.Suppliers.Select(e => e.Company).ToListAsync();
                    ViewData["Materials"] = await context.Materials.ToListAsync();

                    return View(model);
                }
            }
            else
            {
                ViewData["Suppliers"] = await context.Suppliers.Select(e => e.Company).ToListAsync();
                ViewData["Materials"] = await context.Materials.ToListAsync();
                return View(model);
            }
        }

        // GET - Action method for displaying page for editing selected Purchase
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Variable that will hold total ammount of Purchase
            float total = 0;

            // Find Purchase record by id
            Purchase purchaseItem = await context.Purchases
                .Include(e => e.Supplier)
                .Include(e => e.PurchaseDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<PurchaseDetailViewModel>
            List<PurchaseDetailViewModel> detailList = new();

            // Iterate through orderItem.PurchaseDetails and populate detailList
            foreach (var detail in purchaseItem.PurchaseDetails)
            {
                // Declare and instantiate new PurchaseDetailViewModel
                PurchaseDetailViewModel detailViewModelItemItem = new();

                // Set properties
                detailViewModelItemItem.Id = detail.Id;
                detailViewModelItemItem.Purchase = purchaseItem.Code;
                detailViewModelItemItem.Material = detail.Material.Name;
                detailViewModelItemItem.Quantity = detail.Quantity;

                detailList.Add(detailViewModelItemItem);

                total += detail.Material.Price * detail.Quantity;
            }

            // Declare and instantiate new PurchaseViewModel
            PurchaseViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = id;
            viewModelItem.Code = purchaseItem.Code;
            viewModelItem.PurchaseDate = purchaseItem.PurchaseDate;
            viewModelItem.RequiredDate = purchaseItem.RequiredDate;
            viewModelItem.Completed = purchaseItem.Completed;
            viewModelItem.PurchaseDetailList = detailList;

            // Store total variable n ViewData
            ViewData["Total"] = Math.Round(total, 2);
            // Store Supplier company names and Materials in ViewData
            ViewData["Suppliers"] = await context.Suppliers.Select(e => e.Company).ToListAsync();
            ViewData["Materials"] = await context.Materials.ToListAsync();

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // POST - Action method for editing selected Purchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PurchaseViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If model's values are valid
            if (ModelState.IsValid)
            {
                // Find Purchase record by id
                Purchase purchaseItem = context.Purchases
                    .Include(e=>e.Supplier)
                    .Include(e => e.PurchaseDetails)
                    .ThenInclude(e => e.Material)
                    .AsNoTracking()
                    .FirstOrDefault(e => e.Id == model.Id);

                // Check if the detailsList has any elements
                if (model.PurchaseDetailList != null && model.PurchaseDetailList != null && model.PurchaseDetailList.Any() && model.PurchaseDetailList.Where(e => e.Quantity > 0).Count() > 0)
                {
                    // Set properties of purchaseItem
                    purchaseItem.Code = model.Code;
                    purchaseItem.PurchaseDate = model.PurchaseDate;
                    purchaseItem.RequiredDate = model.RequiredDate;
                    purchaseItem.Completed = model.Completed;
                    purchaseItem.Supplier = context.Suppliers.FirstOrDefault(e => e.Company == model.Supplier);

                    // Delete all PurchaseDetail records
                    foreach (var detail in purchaseItem.PurchaseDetails)
                    {
                        detail.Material.InStock -= detail.Quantity;

                        context.PurchaseDetails.Remove(detail);
                    }

                    // Iterate through model.PurchaseDetailList and add PurchaseDetail records to database
                    foreach (var detail in model.PurchaseDetailList)
                    {

                        PurchaseDetail purchaseDetailItem = new();

                        purchaseDetailItem.Purchase = purchaseItem;
                        purchaseDetailItem.Material = context.Materials.FirstOrDefault(e => e.Name == detail.Material);
                        purchaseDetailItem.Quantity = detail.Quantity;

                        Material material = context.Materials.FirstOrDefault(e => e.Name == detail.Material);

                        material.InStock += detail.Quantity;
                    }

                    // Save changes to database
                    await context.SaveChangesAsync();

                    // Redirect to Details page
                    return RedirectToAction(nameof(Details), new { id = purchaseItem.Id });
                }
                // Otherwise return same page with errors
                else
                {
                    ViewData["Error"] = "There must be at least one material in list!";
                    // Store Supplier company names and Materials in ViewData
                    ViewData["Suppliers"] = await context.Suppliers.Select(e => e.Company).ToListAsync();
                    ViewData["Materials"] = await context.Materials.ToListAsync();

                    return View(model);
                }
            }
            else
            {
                ViewData["Suppliers"] = await context.Suppliers.Select(e => e.Company).ToListAsync();
                ViewData["Materials"] = await context.Materials.ToListAsync();
                return View(model);
            }
        }

        // GET - Action method for displaying page with full details for selected Purchase
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Delete(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Variable that will hold total ammount of purchase
            float total = 0;

            // Find Purchase record by id
            Purchase purchaseItem = await context.Purchases
                .Include(e => e.Supplier)
                .Include(e => e.PurchaseDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<PurchaseDetailViewModel>
            List<PurchaseDetailViewModel> detailList = new();

            // Iterate through purchaseItem.PurchaseDetails and populate detailList
            foreach (var detail in purchaseItem.PurchaseDetails)
            {
                // Declare and instantiate new PurchaseDetailViewModel
                PurchaseDetailViewModel detailViewModelItemItem = new();

                // Set properties
                detailViewModelItemItem.Id = detail.Id;
                detailViewModelItemItem.Purchase = purchaseItem.Code;
                detailViewModelItemItem.Material = detail.Material.Name;
                detailViewModelItemItem.Quantity = detail.Quantity;

                detailList.Add(detailViewModelItemItem);

                total += detail.Material.Price * detail.Quantity;
            }

            // Declare and instantiate new PurchaseViewModel
            PurchaseViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = id;
            viewModelItem.Code = purchaseItem.Code;
            viewModelItem.PurchaseDate = purchaseItem.PurchaseDate;
            viewModelItem.RequiredDate = purchaseItem.RequiredDate;
            viewModelItem.Completed = purchaseItem.Completed;
            viewModelItem.PurchaseDetailList = detailList;

            // Store total variable in ViewData
            ViewData["Total"] = Math.Round(total, 2);

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // POST - Action method for deleting selected Purchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(PurchaseViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Purchase record by id
            Purchase purchaseItem = await context.Purchases
                .Include(e => e.PurchaseDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == model.Id);

            // Iterate through purchaseItem.PurchaseDetails and remove all PurchaseDetail records
            foreach (var detail in purchaseItem.PurchaseDetails)
            {
                context.PurchaseDetails.Remove(detail);
            }

            // Remove selected Purchase and save changes to database
            context.Purchases.Remove(purchaseItem);
            await context.SaveChangesAsync();

            // Redirect to Index page
            return RedirectToAction(nameof(Index));
        }
    }
}

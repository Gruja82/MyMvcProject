using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;

namespace MyMvcProject.Controllers
{
    public class ProductionController : Controller
    {
        private readonly IDbContextFactory<DatabaseContext> factory;
        public ProductionController(IDbContextFactory<DatabaseContext> factory)
        {
            this.factory = factory;
        }

        // Helper method for implementing Paging
        private static Pagination<ProductionViewModel> GetPagination(int currentPage, List<ProductionViewModel> viewModelList)
        {
            // Max items shown per page
            int maxRows = 2;

            // Declare and instantiate new Pagination<T>
            Pagination<ProductionViewModel> pagination = new();

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
        // GET - Action method for returning list of all Production records
        [HttpGet]
        [ResponseCache(Duration =60)]
        public async Task<IActionResult> Index()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Declare and instantiate new List<ProductionViewModel>
            List<ProductionViewModel> viewModelList = new();

            // Iterate through all Production records and populate viewModelList
            foreach (var production in context.Production)
            {
                ProductionViewModel viewModelItem = new();

                viewModelItem.Id = production.Id;
                viewModelItem.Code= production.Code;
                viewModelItem.ProductionDate= production.ProductionDate;

                viewModelList.Add(viewModelItem);
            }

            // Return View with Pagination
            return View(GetPagination(1, viewModelList));
        }

        // POST - Action method which is called when Search or Page number button is clicked
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration =60)]
        public async Task<IActionResult> Index(string searchText,int pageIndex = 1)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Load all Production records from database
            List<Production> productionList=await context.Production.ToListAsync();

            // If searchText is not null or empty string then filter the productionList by searchText
            if(searchText!=null && searchText != String.Empty)
            {
                productionList=productionList.Where(e=>e.Code.ToLower().Contains(searchText.ToLower())).ToList();
            }

            // Declare and instantiate new List<ProductionViewModel>
            List<ProductionViewModel> viewModelList = new();

            // Iterate through all Production records and populate viewModelList
            foreach (var production in context.Production)
            {
                ProductionViewModel viewModelItem = new();

                viewModelItem.Id = production.Id;
                viewModelItem.Code = production.Code;
                viewModelItem.ProductionDate = production.ProductionDate;

                viewModelList.Add(viewModelItem);
            }

            // Return View with Pagination
            return View(GetPagination(pageIndex, viewModelList));
        }

        // GET - Action method for displaying page with full details for selected Production
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Details(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Production record by id
            Production productionItem = await context.Production
                .Include(e => e.ProductionDetails)
                .ThenInclude(e=>e.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<ProductionDetailViewModel>
            List<ProductionDetailViewModel> detailViewModelList = new();

            // Iterate through productionItem.ProductionDetails and populate detailViewModelItem
            foreach (var detail in productionItem.ProductionDetails)
            {
                // Declare and instantiate new ProductionDetailViewModel
                ProductionDetailViewModel detailViewModelItem = new();

                detailViewModelItem.Id = detailViewModelItem.Id;
                detailViewModelItem.Production = detail.Production.Code;
                detailViewModelItem.Product = detail.Product.Name;
                detailViewModelItem.Quantity = detail.Quantity;

                detailViewModelList.Add(detailViewModelItem);
            }

            // Declare and instantiate new ProductionViewModel
            ProductionViewModel viewModelItem = new();

            // Set properties of viewModelItem
            viewModelItem.Id = productionItem.Id;
            viewModelItem.Code = productionItem.Code;
            viewModelItem.ProductionDate = productionItem.ProductionDate;
            viewModelItem.ProductionDetailsList = detailViewModelList;

            // Return View viewModelItem
            return View(viewModelItem);
        }

        // GET - Action method for displaying page for creating new Production
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Store Product records in ViewData
            ViewData["Products"] = await context.Products.ToListAsync();

            return View();
        }

        // POST - Action method for creating new Production
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductionViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If all model's values are valid
            if (ModelState.IsValid)
            {
                // Declare and instantiate new Production record
                Production productionItem = new();

                if(model.ProductionDetailsList != null && model.ProductionDetailsList.Any() && model.ProductionDetailsList.Where(e => e.Quantity > 0).Any())
                {
                    // Set properties of productItem
                    productionItem.Code = model.Code;
                    productionItem.ProductionDate = model.ProductionDate;

                    // Add new Production record and save changes to database
                    context.Production.Add(productionItem);
                    await context.SaveChangesAsync();

                    // Iterate through model.ProductionDetailsList and add new ProductionDetail records
                    foreach (var detail in model.ProductionDetailsList)
                    {
                        // Declare nad instantiate new ProductionDetail
                        ProductionDetail productionDetailItem = new();

                        // Set properties
                        productionDetailItem.Product = context.Products.FirstOrDefault(e => e.Name == detail.Product);
                        productionDetailItem.Production = productionItem;
                        productionDetailItem.Quantity= detail.Quantity;

                        // Find Product by name
                        Product productItem = context.Products
                            .Include(e => e.ProductDetails)
                            .ThenInclude(e => e.Material)
                            .FirstOrDefault(e => e.Name == detail.Product);

                        // Increase productItem's quantity
                        productItem.InStock += detail.Quantity;

                        // Decrease Material's quanity needed for production of productItem
                        foreach (var productDetail in productItem.ProductDetails)
                        {
                            // If there is enough Material in stock then reduce material's quantity
                            if(productDetail.Material.InStock>= detail.Quantity * productDetail.Quantity)
                            {
                                productDetail.Material.InStock -= detail.Quantity * productDetail.Quantity;
                            }
                            // Otherwise abort operation and return same page with errors
                            else
                            {
                                context.Production.Remove(productionItem);
                                productItem.InStock -= detail.Quantity;
                                ViewData["Error"] = "There is not enough materials in stock for this production!";
                                // Store Product records in ViewData
                                ViewData["Products"] = await context.Products.ToListAsync();
                                return View(model);
                            }
                        }

                        context.ProductionDetails.Add(productionDetailItem);
                    }

                    await context.SaveChangesAsync();

                    // Redirect to Details page
                    return RedirectToAction(nameof(Details), new { id = productionItem.Id });
                }
                else
                {
                    // Store Product records in ViewData
                    ViewData["Products"] = await context.Products.ToListAsync();
                    ViewData["Error"] = "There must be at least one product in list!";
                    return View(model);
                }
            }
            else
            {
                // Store Product records in ViewData
                ViewData["Products"] = await context.Products.ToListAsync();

                return View(model);
            }
        }

        // GET - Action method for displaying page for editing selected Production
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Production record by id
            Production productionItem = await context.Production
                .Include(e => e.ProductionDetails)
                .ThenInclude(e => e.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<ProductionDetailViewModel>
            List<ProductionDetailViewModel> detailViewModelList = new();

            // Iterate through productionItem.ProductionDetails and populate detailViewModelItem
            foreach (var detail in productionItem.ProductionDetails)
            {
                // Declare and instantiate new ProductionDetailViewModel
                ProductionDetailViewModel detailViewModelItem = new();

                detailViewModelItem.Id = detailViewModelItem.Id;
                detailViewModelItem.Production = detail.Production.Code;
                detailViewModelItem.Product = detail.Product.Name;
                detailViewModelItem.Quantity = detail.Quantity;

                detailViewModelList.Add(detailViewModelItem);
            }

            // Declare and instantiate new ProductionViewModel
            ProductionViewModel viewModelItem = new();

            // Set properties of viewModelItem
            viewModelItem.Id = productionItem.Id;
            viewModelItem.Code = productionItem.Code;
            viewModelItem.ProductionDate = productionItem.ProductionDate;
            viewModelItem.ProductionDetailsList = detailViewModelList;

            // Store Product records in ViewData
            ViewData["Products"] = await context.Products.ToListAsync();

            // Return View viewModelItem
            return View(viewModelItem);
        }

        // POST - Action method for editing selected Production
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductionViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If all model's values are valid
            if (ModelState.IsValid)
            {
                // Find Production record by model's Id
                Production productionItem = context.Production
                    .Include(e => e.ProductionDetails)
                    .ThenInclude(e => e.Product)
                    .ThenInclude(e => e.ProductDetails)
                    .ThenInclude(e => e.Material)
                    .FirstOrDefault(e => e.Id == model.Id);

                if (model.ProductionDetailsList != null && model.ProductionDetailsList.Any() && model.ProductionDetailsList.Where(e => e.Quantity > 0).Any())
                {
                    // Set properties of productItem
                    productionItem.Code = model.Code;
                    productionItem.ProductionDate = model.ProductionDate;

                    // Iterate through model.ProductionDetailsList and add new ProductionDetail records
                    foreach (var detail in model.ProductionDetailsList)
                    {
                        // Remove ProductionDetails records
                        context.ProductionDetails.Remove(context.ProductionDetails.FirstOrDefault(e => e.Production == productionItem));

                        // Find Product record
                        Product productItem = context.Products
                            .Include(e=>e.ProductDetails)
                            .ThenInclude(e=>e.Material)
                            .FirstOrDefault(e => e.Name == detail.Product);

                        // Restore it's quantity
                        productItem.InStock -= detail.Quantity;

                        // Restore Material's quantity
                        foreach (var productDetail in productItem.ProductDetails)
                        {
                            productDetail.Material.InStock+=detail.Quantity*productDetail.Quantity;

                            // If there is enough Material in stock then reduce material's quantity
                            if (productDetail.Material.InStock >= detail.Quantity * productDetail.Quantity)
                            {
                                productDetail.Material.InStock -= detail.Quantity * productDetail.Quantity;
                            }
                            // Otherwise abort operation and return same page with errors
                            else
                            {
                                context.Production.Remove(productionItem);
                                productItem.InStock -= detail.Quantity;

                                ViewData["Error"] = "There is not enough materials in stock for this production!";
                                // Store Product records in ViewData
                                ViewData["Products"] = await context.Products.ToListAsync();
                                return View(model);
                            }
                        }
                        

                    }

                    await context.SaveChangesAsync();

                    // Redirect to Details page
                    return RedirectToAction(nameof(Details), new { id = productionItem.Id });
                }
                else
                {
                    // Store Product records in ViewData
                    ViewData["Products"] = await context.Products.ToListAsync();
                    ViewData["Error"] = "There must be at least one product in list!";
                    return View(model);
                }
            }
            else
            {
                // Store Product records in ViewData
                ViewData["Products"] = await context.Products.ToListAsync();

                return View(model);
            }
        }

        // GET - Action method for displaying page with Production to delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Production record by id
            Production productionItem = await context.Production
                .Include(e => e.ProductionDetails)
                .ThenInclude(e => e.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<ProductionDetailViewModel>
            List<ProductionDetailViewModel> detailViewModelList = new();

            // Iterate through productionItem.ProductionDetails and populate detailViewModelItem
            foreach (var detail in productionItem.ProductionDetails)
            {
                // Declare and instantiate new ProductionDetailViewModel
                ProductionDetailViewModel detailViewModelItem = new();

                detailViewModelItem.Id = detailViewModelItem.Id;
                detailViewModelItem.Production = detail.Production.Code;
                detailViewModelItem.Product = detail.Product.Name;
                detailViewModelItem.Quantity = detail.Quantity;

                detailViewModelList.Add(detailViewModelItem);
            }

            // Declare and instantiate new ProductionViewModel
            ProductionViewModel viewModelItem = new();

            // Set properties of viewModelItem
            viewModelItem.Id = productionItem.Id;
            viewModelItem.Code = productionItem.Code;
            viewModelItem.ProductionDate = productionItem.ProductionDate;
            viewModelItem.ProductionDetailsList = detailViewModelList;

            // Store Product records in ViewData
            ViewData["Products"] = await context.Products.ToListAsync();

            // Return View viewModelItem
            return View(viewModelItem);
        }

        // POST - Action method for deleting selected Production
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProductionViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Production by model's Id
            Production productionItem = await context.Production
                .Include(e => e.ProductionDetails)
                .ThenInclude(e => e.Product)
                .ThenInclude(e => e.ProductDetails)
                .ThenInclude(e => e.Material)
                .FirstOrDefaultAsync(e => e.Id == model.Id);

            // Remove all ProductionDetails records and restore Product and Material quantities
            foreach (var detail in productionItem.ProductionDetails)
            {
                context.ProductionDetails.Remove(detail);
                detail.Product.InStock -= detail.Quantity;

                foreach (var productDetail in detail.Product.ProductDetails)
                {
                    productDetail.Material.InStock += productDetail.Quantity * detail.Quantity;
                }
            }

            // Remove productionItem
            context.Production.Remove(productionItem);

            // Save changes to database
            await context.SaveChangesAsync();

            // Redirect to Index page
            return RedirectToAction(nameof(Index));
        }
    }
}

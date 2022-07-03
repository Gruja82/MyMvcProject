using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;

namespace MyMvcProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly IDbContextFactory<DatabaseContext> factory;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(IDbContextFactory<DatabaseContext> factory, IWebHostEnvironment webHostEnvironment)
        {
            this.factory = factory;
            this.webHostEnvironment = webHostEnvironment;
        }

        // Helper method for implementing Paging
        private static Pagination<ProductViewModel> GetPagination(int currentPage, List<ProductViewModel> viewModelList)
        {
            // Max items shown per page
            int maxRows = 2;

            // Declare and instantiate new Pagination<T>
            Pagination<ProductViewModel> pagination = new();

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
        private string SaveImage(ProductViewModel model)
        {
            string imageFilename = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/Products");
                imageFilename = Guid.NewGuid().ToString().Substring(0, 10) + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, imageFilename);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                model.Image.CopyTo(fileStream);
            }

            return imageFilename;
        }

        // GET - Action method for returning list of all Products
        [HttpGet]
        [ResponseCache(Duration =60)]
        public async Task<IActionResult> Index()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Declare and instantiate new List<ProductViewModel>
            List<ProductViewModel> viewModelList = new();

            // Iterate through all Product records and populate viewModelList
            foreach (var product in context.Products.Include(e=>e.Category).AsNoTracking())
            {
                // Declare and instantiate new ProductViewModel
                ProductViewModel viewModelItem = new();

                // Set properties
                viewModelItem.Id=product.Id;
                viewModelItem.Code = product.Code;
                viewModelItem.Name = product.Name;
                viewModelItem.Category = product.Category.Name;
                viewModelItem.InStock = product.InStock;
                viewModelItem.Price = product.Price;
                viewModelItem.ImagePath = product.ImagePath;

                viewModelList.Add(viewModelItem);
            }

            // Store Category names in ViewData
            ViewData["Categories"] = context.Categories.Select(e => e.Name).ToList();

            // Return View with Pagination
            return View(GetPagination(1, viewModelList));
        }

        // POST - Action method which is called when Search or Page number button is clicked
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration =60)]
        public async Task<IActionResult> Index(string searchText,string category,int pageIndex=1)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Load all Product records from database
            List<Product> productList = context.Products.Include(e => e.Category).ToList();

            // If searchText is not null or empty string then filter the productList by searchText
            if(searchText!=null && searchText != String.Empty)
            {
                productList=productList.Where(
                    e=>e.Code.ToLower().Contains(searchText.ToLower())
                    || e.Name.ToLower().Contains(searchText.ToLower()))
                    .ToList();
            }

            // If category is not equal to it's default value then filter the productList by category
            if(category!="Select category")
            {
                productList=productList.Where(
                    e=>e.Category==context.Categories.FirstOrDefault(e=>e.Name==category))
                    .ToList();
            }

            // Declare and instantiate new List<ProductViewModel>
            List<ProductViewModel> viewModelList = new();

            // Iterate through all Product records and populate viewModelList
            foreach (var product in productList)
            {
                // Declare and instantiate new ProductViewModel
                ProductViewModel viewModelItem = new();

                // Set properties
                viewModelItem.Id = product.Id;
                viewModelItem.Code = product.Code;
                viewModelItem.Name = product.Name;
                viewModelItem.Category = product.Category.Name;
                viewModelItem.InStock = product.InStock;
                viewModelItem.Price = product.Price;
                viewModelItem.ImagePath = product.ImagePath;

                viewModelList.Add(viewModelItem);
            }

            // Store Category names in ViewData
            ViewData["Categories"] = context.Categories.Select(e => e.Name).ToList();

            // Return View with Pagination
            return View(GetPagination(pageIndex, viewModelList));
        }

        

        // GET - Action method for displaying page with full details for selected Product
        [HttpGet]
        [ResponseCache(Duration =60)]
        public async Task<IActionResult> Details(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Variable that will hold production cost for product
            float productionCost = 0;

            // Find Product record by id
            Product productItem = await context.Products
                .Include(e => e.Category)
                .Include(e => e.ProductDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<ProductDetailsViewModel>
            List<ProductDetailsViewModel> detailList = new();

            // Iterate through productItem.ProductDetails and populate detailList
            foreach (var detail in productItem.ProductDetails)
            {
                // Declare and instantiate new ProductDetailsViewModel
                ProductDetailsViewModel detailViewModelItem = new();

                // Set properties
                detailViewModelItem.Id = detailViewModelItem.Id;
                detailViewModelItem.Product = detail.Product.Name;
                detailViewModelItem.Material = detail.Material.Name;
                detailViewModelItem.Quantity = detail.Quantity;

                detailList.Add(detailViewModelItem);

                productionCost += detail.Material.Price * detail.Quantity;
            }

            // Declare and instantiate new ProductViewModel
            ProductViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = productItem.Id;
            viewModelItem.Code = productItem.Code;
            viewModelItem.Name=productItem.Name;
            viewModelItem.Category = productItem.Category.Name;
            viewModelItem.InStock=productItem.InStock;
            viewModelItem.Price = productItem.Price;
            viewModelItem.ImagePath = productItem.ImagePath;
            viewModelItem.ProductDetailsList=detailList;

            // Store productionCost in ViewData
            ViewData["ProductionCost"] = Math.Round(productionCost, 2);

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // GET - Action method for displaying page for creating new Product
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Store Category names and Material records in ViewData
            ViewData["Categories"] = context.Categories.Select(e => e.Name).ToList();
            ViewData["Materials"] = context.Materials.ToList();

            // Return view
            return View();
        }

        // POST - Action method for creating new product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If all model's values are valid
            if (ModelState.IsValid)
            {
                // Declare and instantiate new Product
                Product productItem = new();

                // Check if the ProductDetailsList has any elements
                if(model.ProductDetailsList!=null && model.ProductDetailsList.Any() && model.ProductDetailsList.Where(e => e.Quantity > 0).Any())
                {
                    // Set properties of productItem
                    productItem.Code = model.Code;
                    productItem.Name = model.Name;
                    productItem.Category = context.Categories.FirstOrDefault(e => e.Name == model.Category);
                    productItem.InStock = 0;
                    productItem.Price = model.Price;
                    productItem.ImagePath = SaveImage(model);

                    // Add new Product record and save changes to database
                    context.Products.Add(productItem);
                    await context.SaveChangesAsync();

                    // Iterate through model.ProductDetailsList and add ProductDetail records to database
                    foreach (var detail in model.ProductDetailsList)
                    {
                        ProductDetail productDetailItem = new();

                        productDetailItem.Product = productItem;
                        productDetailItem.Material = context.Materials.FirstOrDefault(e => e.Name == detail.Material);
                        productDetailItem.Quantity = detail.Quantity;

                        await context.ProductDetails.AddAsync(productDetailItem);
                    }

                    // Save changes to database
                    await context.SaveChangesAsync();

                    // Redirect to Details page
                    return RedirectToAction(nameof(Details), new { id = productItem.Id });
                }
                // Otherwise return same page with errors
                else
                {
                    // Store error message, Category names and Material records in ViewData
                    ViewData["Error"] = "There must be at least one material in list!";
                    ViewData["Categories"] = context.Categories.Select(e => e.Name).ToList();
                    ViewData["Materials"] = context.Materials.ToList();

                    return View(model);
                }
            }
            // Otherwise return same page with errors
            else
            {
                // Store Category names and Material records in ViewData
                ViewData["Categories"] = context.Categories.Select(e => e.Name).ToList();
                ViewData["Materials"] = context.Materials.ToList();

                // Return View with model
                return View(model);
            }
        }

        // GET - Action method for displaying page with Product to Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Variable that will hold production cost for product
            float productionCost = 0;

            // Find Product record by id
            Product productItem = await context.Products
                .Include(e => e.Category)
                .Include(e => e.ProductDetails)
                .ThenInclude(e => e.Material)
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<ProductDetailsViewModel>
            List<ProductDetailsViewModel> detailList = new();

            // Iterate through productItem.ProductDetails and populate detailList
            foreach (var detail in productItem.ProductDetails)
            {
                // Declare and instantiate new ProductDetailsViewModel
                ProductDetailsViewModel detailViewModelItem = new();

                // Set properties
                detailViewModelItem.Id = detailViewModelItem.Id;
                detailViewModelItem.Product = detail.Product.Name;
                detailViewModelItem.Material = detail.Material.Name;
                detailViewModelItem.Quantity = detail.Quantity;

                detailList.Add(detailViewModelItem);

                productionCost += detail.Material.Price * detail.Quantity;
            }

            // Declare and instantiate new ProductViewModel
            ProductViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = productItem.Id;
            viewModelItem.Code = productItem.Code;
            viewModelItem.Name = productItem.Name;
            viewModelItem.Category = productItem.Category.Name;
            viewModelItem.InStock = productItem.InStock;
            viewModelItem.Price = productItem.Price;
            viewModelItem.ImagePath = productItem.ImagePath;
            viewModelItem.ProductDetailsList = detailList;

            // Store productionCost in ViewData
            ViewData["ProductionCost"] = Math.Round(productionCost, 2);

            // Store Category names and Material records in ViewData
            ViewData["Categories"] = context.Categories.Select(e => e.Name).ToList();
            ViewData["Materials"] = context.Materials.ToList();

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // POST - Action method for editing selected Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If all model's values are valid
            if (ModelState.IsValid)
            {
                // Find Product by model's Id
                Product productItem = await context.Products
                    .Include(e => e.Category)
                    .Include(e => e.ProductDetails)
                    .ThenInclude(e => e.Material)
                    .FirstOrDefaultAsync(e => e.Id == model.Id);

                // Check if model.ProductDetailsList has any elements
                if(model.ProductDetailsList != null && model.ProductDetailsList.Any() && model.ProductDetailsList.Where(e => e.Quantity > 0).Any())
                {
                    productItem.Code = model.Code;
                    productItem.Name = model.Name;
                    productItem.Category = context.Categories.FirstOrDefault(e => e.Name == model.Category);
                    productItem.InStock = 0;
                    productItem.Price = model.Price;
                    if (model.Image != null)
                    {
                        if (System.IO.File.Exists(Path.Combine(webHostEnvironment.WebRootPath, "images/Products/" + productItem.ImagePath)))
                        {
                            System.IO.File.Delete(Path.Combine(webHostEnvironment.WebRootPath, "images/Products/" + productItem.ImagePath));
                        }

                        productItem.ImagePath = SaveImage(model);
                    }

                    // Delete all productItem.ProductDetail records
                    foreach (var detail in productItem.ProductDetails)
                    {
                        context.ProductDetails.Remove(detail);
                    }

                    // Iterate through model.ProductDetailsList and add ProductDetail records to database
                    foreach (var detail in model.ProductDetailsList)
                    {
                        ProductDetail productDetailItem = new();

                        productDetailItem.Product = productItem;
                        productDetailItem.Material = context.Materials.FirstOrDefault(e => e.Name == detail.Material);
                        productDetailItem.Quantity = detail.Quantity;

                        await context.ProductDetails.AddAsync(productDetailItem);
                    }

                    // Save changes to database
                    await context.SaveChangesAsync();

                    // Redirect to Details page
                    return RedirectToAction(nameof(Details), new { id = productItem.Id });
                }
                // Otherwise return same page with errors
                else
                {
                    // Store error message, Category names and Material records in ViewData
                    ViewData["Error"] = "There must be at least one material in list!";
                    ViewData["Categories"] = context.Categories.Select(e => e.Name).ToList();
                    ViewData["Materials"] = context.Materials.ToList();

                    return View(model);
                }
                
            }
            // Otherwise return same page with errors
            else
            {
                // Store Category names and Material records in ViewData
                ViewData["Categories"] = context.Categories.Select(e => e.Name).ToList();
                ViewData["Materials"] = context.Materials.ToList();

                return View(model);
            }
        }

        // GET - Action method for displaying page for deleting selected Product
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Variable that will hold production cost for product
            float productionCost = 0;

            // Find Product record by id
            Product productItem = await context.Products
                .Include(e => e.Category)
                .Include(e => e.ProductDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<ProductDetailsViewModel>
            List<ProductDetailsViewModel> detailList = new();

            // Iterate through productItem.ProductDetails and populate detailList
            foreach (var detail in productItem.ProductDetails)
            {
                // Declare and instantiate new ProductDetailsViewModel
                ProductDetailsViewModel detailViewModelItem = new();

                // Set properties
                detailViewModelItem.Id = detailViewModelItem.Id;
                detailViewModelItem.Product = detail.Product.Name;
                detailViewModelItem.Material = detail.Material.Name;
                detailViewModelItem.Quantity = detail.Quantity;

                detailList.Add(detailViewModelItem);

                productionCost += detail.Material.Price * detail.Quantity;
            }

            // Declare and instantiate new ProductViewModel
            ProductViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = productItem.Id;
            viewModelItem.Code = productItem.Code;
            viewModelItem.Name = productItem.Name;
            viewModelItem.Category = productItem.Category.Name;
            viewModelItem.InStock = productItem.InStock;
            viewModelItem.Price = productItem.Price;
            viewModelItem.ImagePath = productItem.ImagePath;
            viewModelItem.ProductDetailsList = detailList;

            // Store productionCost in ViewData
            ViewData["ProductionCost"] = Math.Round(productionCost, 2);

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // POST - Action method for deleting selected Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProductViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Product to delete by model's Id
            Product productItem=await context.Products
                .Include(e=>e.ProductDetails)
                .FirstOrDefaultAsync(e=>e.Id==model.Id);

            // Delete all productItem.ProductDetails
            foreach (var detail in productItem.ProductDetails)
            {
                context.ProductDetails.Remove(detail);
            }

            // Remove selected Product and save changes to database
            context.Products.Remove(productItem);
            await context.SaveChangesAsync();

            // Redirect to Index page
            return RedirectToAction(nameof(Index));
        }
    }
}

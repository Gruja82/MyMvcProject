using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Database;
using MyMvcProject.Data.Entities;
using MyMvcProject.Models;

namespace MyMvcProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly IDbContextFactory<DatabaseContext> factory;
        public OrderController(IDbContextFactory<DatabaseContext> factory)
        {
            this.factory = factory;
        }

        // Helper method for implementing Paging
        private static Pagination<OrderViewModel> GetPagination(int currentPage, List<OrderViewModel> viewModelList)
        {
            // Max items shown per page
            int maxRows = 2;

            // Declare and instantiate new Pagination<T>
            Pagination<OrderViewModel> pagination = new();

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


        // GET - Action method for returning all Order records
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Declare and instantiate new List<OrderViewModel>
            List<OrderViewModel> viewModelList = new List<OrderViewModel>();

            // Iterate through all Order records and populate viewModelList
            foreach (var order in context.Orders.Include(e => e.Customer).AsNoTracking())
            {
                // Declare and instantiate new OrderViewModel
                OrderViewModel viewModelItem = new();

                // Set properties
                viewModelItem.Id = order.Id;
                viewModelItem.Code = order.Code;
                viewModelItem.OrderDate = order.OrderDate;
                viewModelItem.RequiredDate = order.RequiredDate;
                viewModelItem.Completed = order.Completed;

                viewModelList.Add(viewModelItem);
            }

            // Store Customer company names in ViewData
            ViewData["Customers"] = await context.Customers.Select(e => e.Company).ToListAsync();

            // Return View with Pagination
            return View(GetPagination(1, viewModelList));
        }

        // POST - Action method which is called when Search or Page number button is clicked
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index(string searchText, string customer, int pageIndex = 1)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Load all orders from database
            List<Order> orderList = await context.Orders.Include(e => e.Customer).ToListAsync();

            // If searchText is not null or empty string then filter the orderList by searchText
            if (searchText != null && searchText != String.Empty)
            {
                orderList = orderList.Where(
                    e => e.Code.ToLower().Contains(searchText.ToLower()))
                    .ToList();
            }

            // If customer is not equal to it's default value then filter the orderList by customer
            if (customer != "Select customer")
            {
                orderList = orderList.Where(e => e.Customer == context.Customers.FirstOrDefault(e => e.Company == customer)).ToList();
            }

            // Declare and instantiate new List<OrderViewModel>
            List<OrderViewModel> viewModelList = new List<OrderViewModel>();

            // Iterate through all Order records and populate viewModelList
            foreach (var order in orderList)
            {
                // Declare and instantiate new OrderViewModel
                OrderViewModel viewModelItem = new();

                // Set properties
                viewModelItem.Id = order.Id;
                viewModelItem.Code = order.Code;
                viewModelItem.OrderDate = order.OrderDate;
                viewModelItem.RequiredDate = order.RequiredDate;
                viewModelItem.Completed = order.Completed;

                viewModelList.Add(viewModelItem);
            }

            // Store Customer company names in ViewData
            ViewData["Customers"] = await context.Customers.Select(e => e.Company).ToListAsync();

            // Return View with Pagination
            return View(GetPagination(pageIndex, viewModelList));
        }

        // GET - Action method for displaying page with full details for selected Order
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Details(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Variable that will hold total ammount of order
            float total = 0;

            // Find Order record by id
            Order orderItem = await context.Orders
                .Include(e=>e.Customer)
                .Include(e => e.OrderDetails)
                .ThenInclude(e => e.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<OrderDetailViewModel>
            List<OrderDetailViewModel> detailList = new();

            // Iterate through orderItem.OrderDetails and populate detailList
            foreach (var detail in orderItem.OrderDetails)
            {
                // Declare and instantiate new OrderDetailViewModel
                OrderDetailViewModel detailViewModelItemItem = new();

                // Set properties
                detailViewModelItemItem.Id = detail.Id;
                detailViewModelItemItem.Order = orderItem.Code;
                detailViewModelItemItem.Product = detail.Product.Name;
                detailViewModelItemItem.Quantity = detail.Quantity;

                detailList.Add(detailViewModelItemItem);

                total += detail.Product.Price * detail.Quantity;
            }

            // Declare and instantiate new OrderViewModel
            OrderViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = id;
            viewModelItem.Code = orderItem.Code;
            viewModelItem.OrderDate = orderItem.OrderDate;
            viewModelItem.RequiredDate = orderItem.RequiredDate;
            viewModelItem.Completed = orderItem.Completed;
            viewModelItem.OrderDetailList = detailList;

            // Store total variable in ViewData
            ViewData["Total"] = Math.Round(total, 2);

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // GET - Action method for displaying page for creating new Order
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Store Customer company names and Products in ViewData
            ViewData["Customers"] = await context.Customers.Select(e => e.Company).ToListAsync();
            ViewData["Products"] = await context.Products.ToListAsync();

            // Return View
            return View();
        }

        // POST - Action method for creating new Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If model's values are valid
            if (ModelState.IsValid)
            {
                // Declare and instantiate new Order
                Order orderItem = new();

                // Check if the OrderDetailList has any elements
                if (model.OrderDetailList!=null && model.OrderDetailList.Any() && model.OrderDetailList.Where(e => e.Quantity > 0).Any())
                {
                    // Set properties of orderItem
                    orderItem.Code = model.Code;
                    orderItem.OrderDate = model.OrderDate;
                    orderItem.RequiredDate = model.RequiredDate;
                    orderItem.Completed = model.Completed;
                    orderItem.Customer = await context.Customers.FirstOrDefaultAsync(e => e.Company == model.Customer);

                    // Add new Order record and save changes to database
                    context.Orders.Add(orderItem);
                    context.SaveChanges();

                    // Iterate through model.OrderDetailList and add OrderDetail records to database
                    foreach (var detail in model.OrderDetailList)
                    {
                        OrderDetail orderDetailItem = new();

                        orderDetailItem.Order = orderItem;
                        orderDetailItem.Product = await context.Products.FirstOrDefaultAsync(e => e.Name == detail.Product);
                        orderDetailItem.Quantity = detail.Quantity;

                        Product product = await context.Products.FirstOrDefaultAsync(e => e.Name == detail.Product);

                        if (product.InStock >= detail.Quantity)
                        {
                            await context.OrderDetails.AddAsync(orderDetailItem);
                            product.InStock -= detail.Quantity;
                        }
                        else
                        {
                            ViewData["Error"] = "There is not enough quantity in stock for those products.";
                            return View(model);
                        }

                    }

                    // Save changes to database
                    await context.SaveChangesAsync();

                    // Redirect to Details page
                    return RedirectToAction(nameof(Details), new { id = orderItem.Id });
                }
                // Otherwise return same page with errors
                else
                {
                    ViewData["Error"] = "There must be at least one product in list!";
                    // Store Customer company names and Products in ViewData
                    ViewData["Customers"] = await context.Customers.Select(e => e.Company).ToListAsync();
                    ViewData["Products"] = await context.Products.ToListAsync();

                    return View(model);
                }
            }
            else
            {
                ViewData["Customers"] = await context.Customers.Select(e => e.Company).ToListAsync();
                ViewData["Products"] = await context.Products.ToListAsync();
                return View(model);
            }
        }

        // GET - Action method for displaying page for editing selected Order
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Variable that will hold total ammount of order
            float total = 0;

            // Find Order record by id
            Order orderItem = await context.Orders
                .Include(e => e.Customer)
                .Include(e => e.OrderDetails)
                .ThenInclude(e => e.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<OrderDetailViewModel>
            List<OrderDetailViewModel> detailList = new();

            // Iterate through orderItem.OrderDetails and populate detailList
            foreach (var detail in orderItem.OrderDetails)
            {
                // Declare and instantiate new OrderDetailViewModel
                OrderDetailViewModel detailViewModelItemItem = new();

                // Set properties
                detailViewModelItemItem.Id = detail.Id;
                detailViewModelItemItem.Order = orderItem.Code;
                detailViewModelItemItem.Product = detail.Product.Name;
                detailViewModelItemItem.Quantity = detail.Quantity;

                detailList.Add(detailViewModelItemItem);

                total += detail.Product.Price * detail.Quantity;
            }

            // Declare and instantiate new OrderViewModel
            OrderViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = id;
            viewModelItem.Code = orderItem.Code;
            viewModelItem.OrderDate = orderItem.OrderDate;
            viewModelItem.RequiredDate = orderItem.RequiredDate;
            viewModelItem.Completed = orderItem.Completed;
            viewModelItem.OrderDetailList = detailList;

            // Store total variable n ViewData
            ViewData["Total"] = Math.Round(total, 2);
            // Store Customer company names and Products in ViewData
            ViewData["Customers"] = await context.Customers.Select(e => e.Company).ToListAsync();
            ViewData["Products"] = await context.Products.ToListAsync();

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // POST - Action method for editing selected Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // If model's values are valid
            if (ModelState.IsValid)
            {
                // Find Order record by id
                Order orderItem = context.Orders
                    .Include(e => e.OrderDetails)
                    .ThenInclude(e => e.Product)
                    .AsNoTracking()
                    .FirstOrDefault(e => e.Id == model.Id);

                // Check if the detailsList has any elements
                if (model.OrderDetailList != null && model.OrderDetailList!=null && model.OrderDetailList.Any() && model.OrderDetailList.Where(e => e.Quantity > 0).Count() > 0)
                {
                    // Set properties of orderItem
                    orderItem.Code = model.Code;
                    orderItem.OrderDate = model.OrderDate;
                    orderItem.RequiredDate = model.RequiredDate;
                    orderItem.Completed = model.Completed;
                    orderItem.Customer = context.Customers.FirstOrDefault(e => e.Company == model.Customer);

                    // Delete all OrderDetail records
                    foreach (var detail in orderItem.OrderDetails)
                    {
                        detail.Product.InStock += detail.Quantity;

                        context.OrderDetails.Remove(detail);
                    }

                    // Iterate through model.OrderDetailList and add OrderDetail records to database
                    foreach (var detail in model.OrderDetailList)
                    {

                        OrderDetail orderDetailItem = new();

                        orderDetailItem.Order = orderItem;
                        orderDetailItem.Product = context.Products.FirstOrDefault(e => e.Name == detail.Product);
                        orderDetailItem.Quantity = detail.Quantity;

                        Product product = context.Products.FirstOrDefault(e => e.Name == detail.Product);

                        if (product.InStock >= detail.Quantity)
                        {
                            await context.OrderDetails.AddAsync(orderDetailItem);
                            product.InStock -= detail.Quantity;
                        }
                        else
                        {
                            ViewData["Error"] = "There is not enough quantity in stock for those products.";
                            return View(model);
                        }

                    }

                    // Save changes to database
                    await context.SaveChangesAsync();

                    // Redirect to Details page
                    return RedirectToAction(nameof(Details), new { id = orderItem.Id });
                }
                // Otherwise return same page with errors
                else
                {
                    ViewData["Error"] = "There must be at least one product in list!";
                    // Store Customer company names and Products in ViewData
                    ViewData["Customers"] = await context.Customers.Select(e => e.Company).ToListAsync();
                    ViewData["Products"] = await context.Products.ToListAsync();

                    return View(model);
                }
            }
            else
            {
                ViewData["Customers"] = await context.Customers.Select(e => e.Company).ToListAsync();
                ViewData["Products"] = await context.Products.ToListAsync();
                return View(model);
            }
        }

        // GET - Action method for displaying page for deleting selected Order
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Variable that will hold total ammount of order
            float total = 0;

            // Find Order record by id
            Order orderItem = await context.Orders
                .Include(e => e.OrderDetails)
                .ThenInclude(e => e.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            // Declare and instantiate new List<OrderDetailViewModel>
            List<OrderDetailViewModel> detailList = new();

            // Iterate through orderItem.OrderDetails and populate detailList
            foreach (var detail in orderItem.OrderDetails)
            {
                // Declare and instantiate new OrderDetailViewModel
                OrderDetailViewModel detailViewModelItemItem = new();

                // Set properties
                detailViewModelItemItem.Id = detail.Id;
                detailViewModelItemItem.Order = orderItem.Code;
                detailViewModelItemItem.Product = detail.Product.Name;
                detailViewModelItemItem.Quantity = detail.Quantity;

                detailList.Add(detailViewModelItemItem);

                total += detail.Product.Price * detail.Quantity;
            }

            // Declare and instantiate new OrderViewModel
            OrderViewModel viewModelItem = new();

            // Set properties
            viewModelItem.Id = id;
            viewModelItem.Code = orderItem.Code;
            viewModelItem.OrderDate = orderItem.OrderDate;
            viewModelItem.RequiredDate = orderItem.RequiredDate;
            viewModelItem.Completed = orderItem.Completed;
            viewModelItem.OrderDetailList = detailList;

            // Store total variable in ViewData
            ViewData["Total"] = Math.Round(total, 2);

            // Return View with viewModelItem
            return View(viewModelItem);
        }

        // POST - Action method for deleting selected Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(OrderViewModel model)
        {
            // Declare new DatabaseContext object
            using var context = await factory.CreateDbContextAsync();

            // Find Order record by id
            Order orderItem = await context.Orders
                .Include(e => e.OrderDetails)
                .ThenInclude(e => e.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == model.Id);

            // Iterate through orderItem.OrderDetails and remove all OrderDetail records
            foreach (var detail in orderItem.OrderDetails)
            {
                context.OrderDetails.Remove(detail);
            }

            // Remove selected Order and save changes to database
            context.Orders.Remove(orderItem);
            await context.SaveChangesAsync();

            // Redirect to Index page
            return RedirectToAction(nameof(Index));
        }
    }
}

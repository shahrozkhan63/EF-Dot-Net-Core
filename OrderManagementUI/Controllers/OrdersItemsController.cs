using Microsoft.AspNetCore.Mvc;
using OrderManagementUI.Models;
using OrderManagementUI.Services;
using OrderManagementUI.ViewModels;

namespace OrderManagementUI.Controllers
{
    public class OrdersItemsController : Controller
    {
        private readonly IOrderService _orderService;

        [BindProperty]
        public OrderViewModel OrderViewModel { get; set; }
        public OrdersItemsController(IOrderService orderService)
        {
            _orderService = orderService;
            OrderViewModel = new OrderViewModel { Order = new Order(), OrderItems = new List<OrderItem>() };
        }

        public async Task<IActionResult> GetOrdersItems()
        {
            // Fetch the list of orders from the service
            var orders = await _orderService.GetOrdersAsync();

            // Initialize an empty list for OrderViewModels
            List<OrderViewModel> orderViewModels = new();

            foreach (var order in orders)
            {
                // Create a new OrderViewModel for each order
                OrderViewModel orderViewModel = new()
                {
                    Order = order,

                    // Convert the ICollection<OrderItem> to List<OrderItem> safely
                    OrderItems = order.OrderItems?.ToList() ?? new List<OrderItem>()
                };

                // Add the mapped view model to the list
                orderViewModels.Add(orderViewModel);
            }

            // Pass the list of OrderViewModels to the view
            return View(orderViewModels);
        }


        public IActionResult CreateOrdersItems()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrdersItems(OrderViewModel orderViewModel)
        {
            Order order = new();
            if (ModelState.IsValid)
            {
                order.OrderNumber = orderViewModel.Order.OrderNumber;
                order.OrderDate = orderViewModel.Order.OrderDate;
                order.CustomerName = orderViewModel.Order.CustomerName;
              

                foreach (var orderItem in orderViewModel.OrderItems)
                {
                    order.OrderItems.Add(orderItem);
                }

                await _orderService.CreateOrderAsync(order);
               
            }

            return RedirectToAction(nameof(GetOrdersItems));
        }

        public async Task<IActionResult> EditOrdersItems(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrdersItems(OrderViewModel orderViewModel)
        {
            Order order = new();
            if (ModelState.IsValid)
            {
                order.OrderId = orderViewModel.Order.OrderId;
                order.OrderNumber = orderViewModel.Order.OrderNumber;
                order.OrderDate = orderViewModel.Order.OrderDate;
                order.CustomerName = orderViewModel.Order.CustomerName;


                foreach (var orderItem in orderViewModel.OrderItems)
                {
                    order.OrderItems.Add(orderItem);
                }

                await _orderService.CreateOrderAsync(order);

            }

            return RedirectToAction(nameof(GetOrdersItems));
        }

        public async Task<IActionResult> DeleteOrdersItems(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmedOrdersItems(int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DetailOrdersItems(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return View(order);
        }
    }
}

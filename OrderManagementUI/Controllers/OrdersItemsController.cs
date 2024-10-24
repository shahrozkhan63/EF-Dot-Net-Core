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
            OrderViewModel = new OrderViewModel { Order = new Order() };
        }

        public async Task<IActionResult> GetOrdersItems()
        {
            // Fetch the list of orders from the service
            var ordersViewModel = await _orderService.GetOrdersAsync();

            // Pass the list of OrderViewModels to the view
            return View(ordersViewModel);
        }


        public IActionResult CreateOrdersItems()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrdersItems(OrderViewModel orderViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();

                ViewBag.Errors = errors;
                TempData["ErrorMessage"] = "Validation failed. Please check the input";
                return View(orderViewModel); // Return the view with the model containing validation errors.
            }

            try
            {
                bool response = await _orderService.CreateOrderAsync(orderViewModel); // Call the service

                if (response)
                {
                    TempData["SuccessMessage"] = "Order created successfully!";
                    return RedirectToAction(nameof(GetOrdersItems)); // Redirect after successful creation
                }
                else
                {
                    TempData["ErrorMessage"] = "failed to add order";
                    return View(orderViewModel); // Return the view with the model containing validation errors.
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message + " | An error occurred while creating the order.";
                return RedirectToAction("Create");
            }
        }


        public async Task<IActionResult> EditOrdersItems(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrdersItems(OrderViewModel orderViewModel)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                    ViewBag.Errors = errors;
                    TempData["ErrorMessage"] = "Validation failed. Please check the input";
                    return View(orderViewModel); // Return the view with the model containing validation errors.
                }


                bool response = await _orderService.CreateOrderAsync(orderViewModel); // Call the service

                if (response)
                {
                    TempData["SuccessMessage"] = "Order updated successfully!";
                    return RedirectToAction(nameof(GetOrdersItems)); // Redirect after successful creation
                }
                else
                {
                    TempData["ErrorMessage"] = "failed to update order";
                    return View(orderViewModel); // Return the view with the model containing validation errors.
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message + " | An error occurred while creating the order.";
                return RedirectToAction("Create");
            }
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

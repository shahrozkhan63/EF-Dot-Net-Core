using Microsoft.AspNetCore.Mvc;
using OrderManagementUI.Models;
using OrderManagementUI.Services;
using OrderManagementUI.ViewModels;

namespace OrderManagementUI.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetOrdersAsync();
            return View(orders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {
                await _orderService.CreateOrderAsync(orderViewModel);
                return RedirectToAction(nameof(Index));
            }
            return View(orderViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {
                await _orderService.UpdateOrderAsync(orderViewModel);
                return RedirectToAction(nameof(Index));
            }
            return View(orderViewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}

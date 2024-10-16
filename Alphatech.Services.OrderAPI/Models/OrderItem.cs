using Alphatech.Services.OrderAPI.Models;

namespace Alphatech.Services.OrderAPI.Models
{
    public class OrderItem
    {
        public int OrderId { get; set; }


        public int ProductId { get; set; }  // No direct reference to Product entity

        public int Quantity { get; set; }

        // Optionally, store some additional product details
        public string ProductName { get; set; }    // Cached product name
        public decimal ProductPrice { get; set; }  // Cached product price
        public Order Order { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace OrderManagementUI.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string? ProductName { get; set; }

        public decimal? ProductPrice { get; set; }

        [JsonIgnore] // Avoid serializing the Order reference to prevent the cycle
        public virtual Order Order { get; set; } = null!;
    }

}

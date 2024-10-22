using System.Text.Json.Serialization;

namespace OrderManagementUI.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string? OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string CustomerName { get; set; } = null!;

        [JsonIgnore] // Avoid serializing the Order reference to prevent the cycle
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

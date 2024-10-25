using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderManagementUI.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string? OrderNumber { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string CustomerName { get; set; } = null!;

        //[JsonIgnore] // Avoid serializing the Order reference to prevent the cycle
        //public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}

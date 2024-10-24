using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderManagementUI.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int Quantity { get; set; }

        [Required]
        public string? ProductName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal? ProductPrice { get; set; }

        //[JsonIgnore] // Avoid serializing the Order reference to prevent the cycle
        //public virtual Order? Order { get; set; } = null!;
    }

}

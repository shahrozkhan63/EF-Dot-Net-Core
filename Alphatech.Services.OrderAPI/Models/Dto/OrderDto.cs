using System.ComponentModel.DataAnnotations;

namespace Alphatech.Services.OrderAPI.Models.Dto
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderNumber { get; set; }
        public string CustomerName { get; set; }

        // Navigation property for the many-to-many relationship
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}

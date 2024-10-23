using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Alphatech.Services.OrderAPI.Models.Dto
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderNumber { get; set; }
        public string CustomerName { get; set; }

        //[JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}

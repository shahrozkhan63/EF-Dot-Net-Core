using System;
using System.Collections.Generic;

namespace Alphatech.Services.OrderAPI.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string? OrderNumber { get; set; }

    public DateTime OrderDate { get; set; }

    public string CustomerName { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

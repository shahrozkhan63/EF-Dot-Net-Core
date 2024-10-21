using System;
using System.Collections.Generic;

namespace Alphatech.Services.OrderAPI.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string? ProductName { get; set; }

    public decimal? ProductPrice { get; set; }

    public virtual Order Order { get; set; } = null!;
}

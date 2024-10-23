using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Alphatech.Services.OrderAPI.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string? ProductName { get; set; }

    public decimal? ProductPrice { get; set; }

    //[JsonIgnore]
    public virtual Order Order { get; set; } = null!;
}

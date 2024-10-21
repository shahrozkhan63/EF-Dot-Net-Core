using System;
using System.Collections.Generic;

namespace Alphatech.Services.ProductAPI.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }
}

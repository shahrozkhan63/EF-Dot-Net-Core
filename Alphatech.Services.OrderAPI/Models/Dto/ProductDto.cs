﻿using System.ComponentModel.DataAnnotations;

namespace Alphatech.Services.OrderAPI.Models.Dto
{
    public class ProductDto
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }

        [Range(1, 1000)]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageURL { get; set; }
    }
}

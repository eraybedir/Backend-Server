﻿using System.ComponentModel.DataAnnotations;

namespace MarketAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required ICollection<Product> Products { get; set; }
    }
}

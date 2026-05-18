using System;
using System.Collections.Generic;

namespace TravelAgency.Database.Models
{
    public class Tour
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string? Hotel { get; set; }
        public string? MealType { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int AvailableSeats { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
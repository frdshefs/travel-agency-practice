using System;
using System.Collections.Generic;

namespace TravelAgency.Database.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string? ClientPhone { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? SpecialRequests { get; set; }
        
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
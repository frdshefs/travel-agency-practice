using System;

namespace TravelAgency.Database.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceAtTime { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime StartDate { get; set; }
        
        public virtual Order? Order { get; set; }
        public virtual Tour? Tour { get; set; }
    }
}
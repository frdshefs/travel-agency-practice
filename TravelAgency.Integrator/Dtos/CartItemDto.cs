using System;

namespace TravelAgency.Integrator.Dtos
{
    public class CartItemDto
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal PricePerTour { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class AddToCartRequest
    {
        public string SessionId { get; set; } = "integration-user";
        public int TourId { get; set; }
        public string TourName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal PricePerTour { get; set; }
    }
}
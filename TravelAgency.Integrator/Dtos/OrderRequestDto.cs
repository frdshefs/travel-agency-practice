using System;

namespace TravelAgency.Integrator.Dtos
{
    public class OrderRequestDto
    {
        public string ClientName { get; set; } = "";
        public string ClientEmail { get; set; } = "";
        public string ClientPhone { get; set; } = "";
        public int TourId { get; set; }
        public string TourName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string StartDate { get; set; } = "";
    }

    public class OrderResponseDto
    {
        public string Message { get; set; } = "";
        public OrderDto Order { get; set; } = new OrderDto();
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = "";
        public string ClientEmail { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
using System;

namespace TravelAgency.Integrator.Dtos
{
    public class TourDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Destination { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public bool Available { get; set; }
    }
}
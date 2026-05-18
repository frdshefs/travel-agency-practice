using TravelAgency.Integrator.Dtos;

namespace TravelAgency.Integrator.Adapters
{
    public class TourToCartAdapter : ITourToCartAdapter
    {
        public AddToCartRequest Transform(TourDto tour, int quantity)
        {
            return new AddToCartRequest
            {
                SessionId = "integration-session",
                TourId = tour.Id,
                TourName = tour.Name ?? "",
                Quantity = quantity,
                PricePerTour = tour.Price
            };
        }
    }
}
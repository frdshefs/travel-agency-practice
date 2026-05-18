using TravelAgency.Integrator.Dtos;

namespace TravelAgency.Integrator.Adapters
{
    public interface ITourToCartAdapter
    {
        AddToCartRequest Transform(TourDto tour, int quantity);
    }

    public interface ICartToOrderAdapter
    {
        OrderRequestDto Transform(CartItemDto cartItem, string clientName, string clientEmail);
    }
}
using System;
using TravelAgency.Integrator.Dtos;

namespace TravelAgency.Integrator.Adapters
{
    public class CartToOrderAdapter : ICartToOrderAdapter
    {
        public OrderRequestDto Transform(CartItemDto cartItem, string clientName, string clientEmail)
        {
            return new OrderRequestDto
            {
                ClientName = clientName,
                ClientEmail = clientEmail,
                ClientPhone = "",
                TourId = cartItem.TourId,
                TourName = cartItem.TourName,
                Quantity = cartItem.Quantity,
                TotalPrice = cartItem.TotalPrice,
                StartDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd")
            };
        }
    }
}
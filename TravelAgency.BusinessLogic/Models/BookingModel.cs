using System;

namespace TravelAgency.BusinessLogic.Models
{
    public class BookingModel
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string PaymentTransactionId { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public enum BookingStatus
    {
        New,           // Новое бронирование
        Reserved,      // Места зарезервированы
        Paid,          // Оплачено
        Confirmed,     // Подтверждено
        Cancelled,     // Отменено
        Failed         // Ошибка
    }
}
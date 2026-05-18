using System;
using Microsoft.Extensions.Logging;
using TravelAgency.BusinessLogic.Models;

namespace TravelAgency.BusinessLogic.StateMachines
{
    public class BookingStateMachine
    {
        private readonly ILogger<BookingStateMachine> _logger;
        private BookingStatus _currentStatus;

        public BookingStateMachine(ILogger<BookingStateMachine> logger)
        {
            _logger = logger;
            _currentStatus = BookingStatus.New;
        }

        public BookingStatus CurrentStatus => _currentStatus;

        public bool CanReserve()
        {
            return _currentStatus == BookingStatus.New;
        }

        public bool CanPay()
        {
            return _currentStatus == BookingStatus.Reserved;
        }

        public bool CanConfirm()
        {
            return _currentStatus == BookingStatus.Paid;
        }

        public bool CanCancel()
        {
            return _currentStatus == BookingStatus.Reserved || _currentStatus == BookingStatus.Paid;
        }

        public void Reserve()
        {
            if (!CanReserve())
                throw new InvalidOperationException($"Невозможно зарезервировать из статуса {_currentStatus}");
            
            _currentStatus = BookingStatus.Reserved;
            _logger.LogInformation("Статус бронирования: New → Reserved");
        }

        public void Pay()
        {
            if (!CanPay())
                throw new InvalidOperationException($"Невозможно оплатить из статуса {_currentStatus}");
            
            _currentStatus = BookingStatus.Paid;
            _logger.LogInformation("Статус бронирования: Reserved → Paid");
        }

        public void Confirm()
        {
            if (!CanConfirm())
                throw new InvalidOperationException($"Невозможно подтвердить из статуса {_currentStatus}");
            
            _currentStatus = BookingStatus.Confirmed;
            _logger.LogInformation("Статус бронирования: Paid → Confirmed");
        }

        public void Cancel()
        {
            if (!CanCancel())
                throw new InvalidOperationException($"Невозможно отменить из статуса {_currentStatus}");
            
            _currentStatus = BookingStatus.Cancelled;
            _logger.LogInformation("Статус бронирования: {PreviousStatus} → Cancelled", _currentStatus);
        }

        public void Fail()
        {
            _currentStatus = BookingStatus.Failed;
            _logger.LogWarning("Статус бронирования → Failed");
        }
    }
}
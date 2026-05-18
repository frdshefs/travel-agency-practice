using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using TravelAgency.BusinessLogic.Models;
using TravelAgency.BusinessLogic.StateMachines;

namespace TravelAgency.BusinessLogic.Services
{
    public interface IBookingOrchestrator
    {
        Task<BookingResult> ProcessBookingAsync(BookingRequest request);
    }

    public class BookingRequest
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
    }

    public class BookingResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public BookingModel? Booking { get; set; }
    }

    public class BookingOrchestrator : IBookingOrchestrator
    {
        private readonly IStockService _stockService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<BookingOrchestrator> _logger;

        public BookingOrchestrator(
            IStockService stockService,
            IPaymentService paymentService,
            ILogger<BookingOrchestrator> logger)
        {
            _stockService = stockService;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<BookingResult> ProcessBookingAsync(BookingRequest request)
        {
            _logger.LogInformation("=== НАЧАЛО БРОНИРОВАНИЯ ===");
            _logger.LogInformation("Тур: {TourName}, Количество: {Quantity}, Сумма: {Total}", 
                request.TourName, request.Quantity, request.Quantity * request.Price);

            var booking = new BookingModel
            {
                TourId = request.TourId,
                TourName = request.TourName,
                Quantity = request.Quantity,
                TotalPrice = request.Quantity * request.Price,
                ClientName = request.ClientName,
                ClientEmail = request.ClientEmail,
                ClientPhone = request.ClientPhone,
                Status = BookingStatus.New,
                CreatedAt = DateTime.Now
            };

            // Создаём отдельный логгер для StateMachine
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var stateMachineLogger = loggerFactory.CreateLogger<BookingStateMachine>();
            var stateMachine = new BookingStateMachine(stateMachineLogger);

            try
            {
                // Шаг 1: Проверка остатков
                _logger.LogInformation("Шаг 1: Проверка остатков...");
                var stockAvailable = await _stockService.CheckStockAsync(request.TourId, request.Quantity);
                
                if (!stockAvailable)
                {
                    stateMachine.Fail();
                    booking.Status = BookingStatus.Failed;
                    booking.ErrorMessage = "Недостаточно мест";
                    return new BookingResult { IsSuccess = false, Message = "Недостаточно мест", Booking = booking };
                }

                // Шаг 2: Резервирование мест
                _logger.LogInformation("Шаг 2: Резервирование мест...");
                var reserveResult = await _stockService.ReserveStockAsync(request.TourId, request.Quantity);
                
                if (!reserveResult)
                {
                    stateMachine.Fail();
                    booking.Status = BookingStatus.Failed;
                    booking.ErrorMessage = "Ошибка резервирования";
                    return new BookingResult { IsSuccess = false, Message = "Ошибка резервирования", Booking = booking };
                }
                
                stateMachine.Reserve();
                booking.Status = BookingStatus.Reserved;

                // Шаг 3: Обработка платежа с Polly (Retry)
                _logger.LogInformation("Шаг 3: Обработка платежа...");
                
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.LogWarning("Попытка {RetryCount} оплаты не удалась. Повтор через {TimeSpan}", retryCount, timeSpan);
                        });

                var paymentResult = await retryPolicy.ExecuteAsync(async () => 
                    await _paymentService.ProcessPaymentAsync(booking.TotalPrice, request.ClientName, request.ClientEmail));

                if (!paymentResult.IsSuccess)
                {
                    // Компенсация: освобождаем места
                    _logger.LogWarning("Платеж не удался. Запуск компенсации...");
                    await _stockService.ReleaseStockAsync(request.TourId, request.Quantity);
                    stateMachine.Cancel();
                    booking.Status = BookingStatus.Cancelled;
                    booking.ErrorMessage = paymentResult.ErrorMessage;
                    return new BookingResult { IsSuccess = false, Message = "Ошибка оплаты", Booking = booking };
                }

                booking.PaymentTransactionId = paymentResult.TransactionId;
                stateMachine.Pay();
                booking.Status = BookingStatus.Paid;
                booking.PaidAt = DateTime.Now;

                // Шаг 4: Подтверждение бронирования
                _logger.LogInformation("Шаг 4: Подтверждение бронирования...");
                stateMachine.Confirm();
                booking.Status = BookingStatus.Confirmed;
                booking.ConfirmedAt = DateTime.Now;

                _logger.LogInformation("=== БРОНИРОВАНИЕ УСПЕШНО ЗАВЕРШЕНО! ===");
                _logger.LogInformation("Booking Status: {Status}", booking.Status);

                return new BookingResult 
                { 
                    IsSuccess = true, 
                    Message = "Бронирование успешно оформлено!", 
                    Booking = booking 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка при бронировании");
                
                // Компенсация
                if (booking.Status == BookingStatus.Reserved)
                {
                    await _stockService.ReleaseStockAsync(request.TourId, request.Quantity);
                }
                
                stateMachine.Fail();
                booking.Status = BookingStatus.Failed;
                booking.ErrorMessage = ex.Message;
                
                return new BookingResult { IsSuccess = false, Message = $"Ошибка: {ex.Message}", Booking = booking };
            }
        }
    }
}
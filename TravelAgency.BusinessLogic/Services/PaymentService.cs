using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TravelAgency.BusinessLogic.Services
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(decimal amount, string clientName, string clientEmail);
        Task<bool> RefundPaymentAsync(string transactionId);
    }

    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private static readonly Random _random = new();

        public PaymentService(ILogger<PaymentService> logger)
        {
            _logger = logger;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, string clientName, string clientEmail)
        {
            _logger.LogInformation("Обработка платежа на сумму: {Amount} руб. для {ClientName}", amount, clientName);
            
            await Task.Delay(500); // Имитация обработки платежа
            
            // Для демонстрации: 90% успешных платежей
            var isSuccess = _random.Next(1, 101) <= 90;
            
            if (isSuccess)
            {
                var transactionId = $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{_random.Next(1000, 9999)}";
                _logger.LogInformation("Платеж успешен! Transaction ID: {TransactionId}", transactionId);
                return new PaymentResult { IsSuccess = true, TransactionId = transactionId };
            }
            else
            {
                _logger.LogError("Платеж отклонен!");
                return new PaymentResult { IsSuccess = false, ErrorMessage = "Платеж отклонен банком" };
            }
        }

        public async Task<bool> RefundPaymentAsync(string transactionId)
        {
            _logger.LogInformation("Возврат платежа по транзакции: {TransactionId}", transactionId);
            await Task.Delay(200);
            return await Task.FromResult(true);
        }
    }
}
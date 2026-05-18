using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TravelAgency.BusinessLogic.Services
{
    public interface IStockService
    {
        Task<bool> CheckStockAsync(int tourId, int quantity);
        Task<bool> ReserveStockAsync(int tourId, int quantity);
        Task<bool> ReleaseStockAsync(int tourId, int quantity);
    }

    public class StockService : IStockService
    {
        private readonly ILogger<StockService> _logger;

        public StockService(ILogger<StockService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> CheckStockAsync(int tourId, int quantity)
        {
            _logger.LogInformation("Проверка остатков для тура ID: {TourId}, количество: {Quantity}", tourId, quantity);
            
            // Имитация проверки (в реальном проекте - вызов к БД или API модуля А)
            await Task.Delay(100);
            
            // Для примера: тур 1 имеет 25 мест, тур 2 - 30 мест, тур 3 - 0 мест
            var availableSeats = tourId switch
            {
                1 => 25,
                2 => 30,
                3 => 0,
                4 => 10,
                _ => 0
            };
            
            var isAvailable = availableSeats >= quantity;
            
            if (isAvailable)
                _logger.LogInformation("Достаточно мест! Доступно: {Available}, запрошено: {Quantity}", availableSeats, quantity);
            else
                _logger.LogWarning("Недостаточно мест! Доступно: {Available}, запрошено: {Quantity}", availableSeats, quantity);
            
            return await Task.FromResult(isAvailable);
        }

        public async Task<bool> ReserveStockAsync(int tourId, int quantity)
        {
            _logger.LogInformation("Резервирование мест для тура ID: {TourId}, количество: {Quantity}", tourId, quantity);
            await Task.Delay(100);
            return await Task.FromResult(true);
        }

        public async Task<bool> ReleaseStockAsync(int tourId, int quantity)
        {
            _logger.LogInformation("Освобождение мест для тура ID: {TourId}, количество: {Quantity}", tourId, quantity);
            await Task.Delay(100);
            return await Task.FromResult(true);
        }
    }
}
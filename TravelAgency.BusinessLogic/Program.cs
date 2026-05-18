using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TravelAgency.BusinessLogic.Services;
using TravelAgency.BusinessLogic.Models;

Console.WriteLine("=== Туристическое агентство - Сквозной процесс бронирования ===\n");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton<IStockService, StockService>();
        services.AddSingleton<IPaymentService, PaymentService>();
        services.AddSingleton<IBookingOrchestrator, BookingOrchestrator>();
    })
    .Build();

var orchestrator = host.Services.GetRequiredService<IBookingOrchestrator>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

// Тестовый сценарий 1: Успешное бронирование
logger.LogInformation("\n========== ТЕСТ 1: УСПЕШНОЕ БРОНИРОВАНИЕ ==========\n");

var request1 = new BookingRequest
{
    TourId = 1,
    TourName = "Отдых в Турции",
    Quantity = 2,
    Price = 45000,
    ClientName = "Иван Петров",
    ClientEmail = "ivan@example.com",
    ClientPhone = "+79991234567"
};

var result1 = await orchestrator.ProcessBookingAsync(request1);
Console.WriteLine($"\nРезультат: {result1.Message}");
if (result1.Booking != null)
{
    Console.WriteLine($"Booking Status: {result1.Booking.Status}");
    Console.WriteLine($"Transaction ID: {result1.Booking.PaymentTransactionId}");
}

// Тестовый сценарий 2: Недостаточно мест
logger.LogInformation("\n========== ТЕСТ 2: НЕДОСТАТОЧНО МЕСТ ==========\n");

var request2 = new BookingRequest
{
    TourId = 3,
    TourName = "Путешествие на Байкал",
    Quantity = 5,
    Price = 35000,
    ClientName = "Петр Сидоров",
    ClientEmail = "petr@example.com",
    ClientPhone = "+79997654321"
};

var result2 = await orchestrator.ProcessBookingAsync(request2);
Console.WriteLine($"\nРезультат: {result2.Message}");

// Тестовый сценарий 3: Ошибка оплаты (будет компенсация)
logger.LogInformation("\n========== ТЕСТ 3: ОШИБКА ОПЛАТЫ (КОМПЕНСАЦИЯ) ==========\n");

var request3 = new BookingRequest
{
    TourId = 2,
    TourName = "Экскурсии в Питер",
    Quantity = 1,
    Price = 15000,
    ClientName = "Мария Иванова",
    ClientEmail = "maria@example.com",
    ClientPhone = "+79991112233"
};

// Запускаем несколько раз для демонстрации
for (int i = 0; i < 5; i++)
{
    var result3 = await orchestrator.ProcessBookingAsync(request3);
    if (!result3.IsSuccess)
    {
        Console.WriteLine($"\nРезультат: {result3.Message}");
        break;
    }
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();
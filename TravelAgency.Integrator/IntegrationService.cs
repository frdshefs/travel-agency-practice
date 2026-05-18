using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace TravelAgency.Integrator
{
    public class IntegrationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IntegrationService> _logger;

        public IntegrationService(HttpClient httpClient, ILogger<IntegrationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> ProcessTourBookingAsync(int tourId, int quantity, string clientName, string clientEmail)
        {
            _logger.LogInformation("Начинаем бронирование тура ID: {TourId} для {ClientName}", tourId, clientName);

            try
            {
                // Шаг 1: Получить тур из каталога (Модуль А)
                _logger.LogInformation("Шаг 1: Получение тура из каталога...");
                var tour = await GetTourFromCatalogAsync(tourId);
                if (tour == null)
                {
                    _logger.LogError("Тур с ID {TourId} не найден", tourId);
                    return false;
                }
                _logger.LogInformation("Тур найден: {TourName} - {Price} руб.", tour.Name, tour.Price);

                // Шаг 2: Добавить в корзину (Модуль Б)
                _logger.LogInformation("Шаг 2: Добавление тура в корзину...");
                var cartItem = await AddToCartAsync(tourId, tour.Name, quantity, tour.Price);
                if (cartItem == null)
                {
                    _logger.LogError("Не удалось добавить тур в корзину");
                    return false;
                }
                _logger.LogInformation("Тур добавлен в корзину. Сумма: {TotalPrice} руб.", cartItem.TotalPrice);

                // Шаг 3: Создать заказ (Модуль В)
                _logger.LogInformation("Шаг 3: Оформление заказа...");
                var order = await CreateOrderAsync(clientName, clientEmail, tourId, tour.Name, quantity, cartItem.TotalPrice);
                if (order == null)
                {
                    _logger.LogError("Не удалось создать заказ");
                    return false;
                }

                _logger.LogInformation("УСПЕХ! Заказ #{OrderId} создан. Статус: {Status}", order.Id, order.Status);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при бронировании тура");
                return false;
            }
        }

        private async Task<TourDto?> GetTourFromCatalogAsync(int tourId)
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5001/products");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Ошибка HTTP: {StatusCode}", response.StatusCode);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Получен JSON каталога");
                
                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true
                };
                
                var tours = JsonSerializer.Deserialize<List<TourDto>>(json, options);
                
                if (tours == null || !tours.Any())
                {
                    _logger.LogError("Список туров пуст");
                    return null;
                }
                
                var tour = tours.FirstOrDefault(t => t.Id == tourId);
                
                if (tour != null)
                {
                    _logger.LogInformation("Найден тур ID: {Id}, Name: {Name}", tour.Id, tour.Name);
                }
                else
                {
                    _logger.LogInformation("Доступные ID: {Ids}", string.Join(", ", tours.Select(t => t.Id)));
                }
                
                return tour;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении тура");
                return null;
            }
        }

        private async Task<CartItemDto?> AddToCartAsync(int tourId, string? tourName, int quantity, decimal price)
        {
            try
            {
                var cartData = new
                {
                    SessionId = "integration-session",
                    TourId = tourId,
                    TourName = tourName ?? "Тур",
                    Quantity = quantity,
                    PricePerTour = price
                };

                var content = new StringContent(JsonSerializer.Serialize(cartData), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:5002/cart/add", content);

                if (!response.IsSuccessStatusCode)
                    return null;

                return new CartItemDto
                {
                    TourId = tourId,
                    TourName = tourName ?? "Тур",
                    Quantity = quantity,
                    PricePerTour = price,
                    TotalPrice = quantity * price
                };
            }
            catch
            {
                return null;
            }
        }

        private async Task<OrderDto?> CreateOrderAsync(string clientName, string clientEmail, int tourId, string? tourName, int quantity, decimal totalPrice)
        {
            try
            {
                var orderData = new
                {
                    clientName = clientName,
                    clientEmail = clientEmail,
                    tourId = tourId,
                    tourName = tourName ?? "Тур",
                    quantity = quantity,
                    totalPrice = totalPrice
                };

                var content = new StringContent(JsonSerializer.Serialize(orderData), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:3000/orders/create", content);

                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Ответ от сервера заказов: {Response}", responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("HTTP ошибка: {StatusCode}", response.StatusCode);
                    return null;
                }

                using var doc = JsonDocument.Parse(responseBody);
                
                // Ищем поле "booking" (основной формат)
                if (doc.RootElement.TryGetProperty("booking", out var bookingElement))
                {
                    var id = bookingElement.GetProperty("id").GetInt32();
                    var status = bookingElement.GetProperty("status").GetString() ?? "confirmed";
                    
                    _logger.LogInformation("Бронирование создано с ID: {BookingId}, статус: {Status}", id, status);
                    
                    return new OrderDto
                    {
                        Id = id,
                        ClientName = clientName,
                        ClientEmail = clientEmail,
                        Status = status,
                        CreatedAt = DateTime.Now
                    };
                }
                // Ищем поле "order" (альтернативный формат)
                else if (doc.RootElement.TryGetProperty("order", out var orderElement))
                {
                    var id = orderElement.GetProperty("id").GetInt32();
                    var status = orderElement.GetProperty("status").GetString() ?? "created";
                    
                    _logger.LogInformation("Заказ создан с ID: {OrderId}, статус: {Status}", id, status);
                    
                    return new OrderDto
                    {
                        Id = id,
                        ClientName = clientName,
                        ClientEmail = clientEmail,
                        Status = status,
                        CreatedAt = DateTime.Now
                    };
                }
                // Ищем поле "id" (прямой ответ)
                else if (doc.RootElement.TryGetProperty("id", out var idElement))
                {
                    var id = idElement.GetInt32();
                    var status = doc.RootElement.TryGetProperty("status", out var statusElement) 
                        ? statusElement.GetString() ?? "created" 
                        : "created";
                    
                    _logger.LogInformation("Заказ создан с ID: {OrderId}, статус: {Status}", id, status);
                    
                    return new OrderDto
                    {
                        Id = id,
                        ClientName = clientName,
                        ClientEmail = clientEmail,
                        Status = status,
                        CreatedAt = DateTime.Now
                    };
                }
                else
                {
                    _logger.LogError("Неизвестный формат ответа: {Response}", responseBody);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании заказа");
                return null;
            }
        }
    }

    public class TourDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("destination")]
        public string? Destination { get; set; }
        
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        [JsonPropertyName("duration")]
        public string? Duration { get; set; }
        
        [JsonPropertyName("available")]
        public bool Available { get; set; }
        
        [JsonPropertyName("hotel")]
        public string? Hotel { get; set; }
    }

    public class CartItemDto
    {
        public int TourId { get; set; }
        public string? TourName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerTour { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public string? ClientName { get; set; }
        public string? ClientEmail { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
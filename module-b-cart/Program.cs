var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Хранилище корзин: sessionId -> список туров
Dictionary<string, List<CartItem>> carts = new();

// Эндпоинт для добавления тура в корзину (основной по заданию)
app.MapPost("/cart/add", (AddToCartRequest request) =>
{
    string sessionId = request.SessionId ?? "guest";

    if (!carts.ContainsKey(sessionId))
        carts[sessionId] = new List<CartItem>();

    var existingItem = carts[sessionId].FirstOrDefault(i => i.TourId == request.TourId);

    if (existingItem != null)
    {
        existingItem.Quantity += request.Quantity;
        existingItem.TotalPrice = existingItem.Quantity * existingItem.PricePerTour;
    }
    else
    {
        carts[sessionId].Add(new CartItem
        {
            TourId = request.TourId,
            TourName = request.TourName,
            Quantity = request.Quantity,
            PricePerTour = request.PricePerTour,
            TotalPrice = request.Quantity * request.PricePerTour
        });
    }

    return Results.Ok(new {
        message = "Тур добавлен в корзину",
        cart = carts[sessionId],
        total = carts[sessionId].Sum(i => i.TotalPrice)
    });
});

// Доп. эндпоинт, чтобы проверять содержимое корзины (удобно для тестов)
app.MapGet("/cart/{sessionId}", (string sessionId) =>
{
    if (carts.ContainsKey(sessionId))
        return Results.Ok(new { cart = carts[sessionId], total = carts[sessionId].Sum(i => i.TotalPrice) });

    return Results.Ok(new { cart = new List<CartItem>(), total = 0 });
});

app.Run("http://localhost:5002");

// Модели данных
record CartItem
{
    public int TourId { get; set; }
    public string TourName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal PricePerTour { get; set; }
    public decimal TotalPrice { get; set; }
}

record AddToCartRequest
{
    public string? SessionId { get; set; }
    public int TourId { get; set; }
    public string TourName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal PricePerTour { get; set; }
}
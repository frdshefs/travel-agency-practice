const express = require('express');
const app = express();
app.use(express.json());

let orders = [];

app.post('/orders/create', (req, res) => {
    console.log('Получен запрос:', req.body);
    
    const { clientName, clientEmail, tourId, tourName, quantity, totalPrice } = req.body;
    
    const newOrder = {
        id: orders.length + 1,
        clientName: clientName || 'Не указан',
        clientEmail: clientEmail || 'Не указан',
        tourId: tourId || 0,
        tourName: tourName || 'Не указан',
        quantity: quantity || 1,
        totalPrice: totalPrice || 0,
        status: 'created',
        createdAt: new Date()
    };
    
    orders.push(newOrder);
    res.json({ message: 'Заказ создан', order: newOrder });
});

app.get('/orders', (req, res) => {
    res.json(orders);
});

app.listen(3000, () => {
    console.log('Orders module running on port 3000');
});
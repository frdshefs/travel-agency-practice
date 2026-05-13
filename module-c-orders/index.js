const express = require('express');
const app = express();
app.use(express.json());

let bookings = [];

app.post('/orders/create', (req, res) => {
    const { clientName, clientEmail, tourId, tourName, quantity, totalPrice } = req.body;

    const booking = {
        id: bookings.length + 1,
        clientName: clientName,
        clientEmail: clientEmail,
        tour: {
            id: tourId,
            name: tourName,
            quantity: quantity,
            totalPrice: totalPrice
        },
        status: 'confirmed',
        createdAt: new Date()
    };

    bookings.push(booking);
    res.json({ message: 'Бронирование создано!', booking: booking });
});

app.get('/orders', (req, res) => {
    res.json(bookings);
});

app.listen(3000, () => {
    console.log('Модуль бронирований работает на порту 3000');
});
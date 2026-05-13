from flask import Flask, jsonify

app = Flask(__name__)

tours = [
    {
        "id": 1,
        "name": "Отдых в Турции",
        "destination": "Анталья",
        "duration": "7 ночей",
        "price": 45000,
        "hotel": "5* All Inclusive",
        "available": True
    },
    {
        "id": 2,
        "name": "Экскурсии в Санкт-Петербург",
        "destination": "Санкт-Петербург",
        "duration": "3 дня",
        "price": 15000,
        "hotel": "3* Завтраки",
        "available": True
    },
    {
        "id": 3,
        "name": "Путешествие на Байкал",
        "destination": "Иркутск",
        "duration": "5 дней",
        "price": 35000,
        "hotel": "Гостевой дом",
        "available": False
    },
    {
        "id": 4,
        "name": "Новый год в Лапландии",
        "destination": "Финляндия",
        "duration": "4 ночи",
        "price": 89000,
        "hotel": "Коттедж",
        "available": True
    }
]


@app.route('/products', methods=['GET'])
def get_all_tours():
    """Получить все туры"""
    return jsonify(tours)


@app.route('/tours/<int:tour_id>', methods=['GET'])
def get_tour(tour_id):
    """Получить тур по ID"""
    tour = next((t for t in tours if t["id"] == tour_id), None)
    if tour:
        return jsonify(tour)
    return jsonify({"error": "Тур не найден"}), 404


@app.route('/tours/available', methods=['GET'])
def get_available_tours():
    """Получить только доступные туры"""
    available = [t for t in tours if t["available"]]
    return jsonify(available)


if __name__ == '__main__':
    app.run(port=5001)
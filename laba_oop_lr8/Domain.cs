using System;
using System.Collections.Generic;
using System.Linq;

namespace laba_oop_lr8
{
    /// <summary>
    /// Визначає контракт для об'єктів, які підтримують пошук за ключовим словом.
    /// Демонструє відношення: Реалізація (Realization).
    /// </summary>
    public interface ISearchable
    {
        /// <summary>
        /// Перевіряє, чи відповідає об'єкт заданому ключовому слову.
        /// </summary>
        bool Matches(string keyword);
    }

    /// <summary>
    /// Кастомний клас виключення для предметної галузі готелю.
    /// Демонструє обробку виключних ситуацій.
    /// </summary>
    public class BookingException : Exception
    {
        /// <summary>
        /// Ініціалізує новий екземпляр класу виключення із заданим повідомленням.
        /// </summary>
        public BookingException(string message) : base(message) { }
    }

    /// <summary>
    /// Абстрактний базовий клас для всіх фізичних осіб.
    /// Демонструє принципи: Абстракція.
    /// </summary>
    public abstract class Person : ISearchable
    {
        /// <summary>
        /// Ім'я особи.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Прізвище особи.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Абстрактний метод для отримання опису особи.
        /// Демонструє поліморфізм.
        /// </summary>
        public abstract string GetDescription();

        /// <summary>
        /// Перевіряє наявність ключового слова в імені або прізвищі.
        /// </summary>
        public virtual bool Matches(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return false;
            return FirstName.Contains(keyword, StringComparison.OrdinalIgnoreCase) || LastName.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Представляє клієнта системи бронювання.
    /// Демонструє відношення: Узагальнення (Generalization) - спадкування від Person.
    /// </summary>
    public class Client : Person
    {
        private static int _nextId = 1; 

        /// <summary>
        /// Унікальний цілочисельний ідентифікатор клієнта.
        /// </summary>
        public int Id { get; private set; }

        public string PhoneNumber { get; set; }

        public Client(string firstName, string lastName, string phoneNumber)
        {
            Id = _nextId++; 
            
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }

        public override string GetDescription()
        {
            return $"Client [ID: {Id}]: {FirstName} {LastName}, Phone: {PhoneNumber}";
        }
    }

    /// <summary>
    /// Представляє кімнату в готелі.
    /// Існує лише в контексті готелю (Композиція).
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Номер кімнати.
        /// </summary>
        public string RoomNumber { get; set; }

        /// <summary>
        /// Місткість (кількість місць).
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Вартість за одну добу.
        /// </summary>
        public decimal PricePerNight { get; set; }

        /// <summary>
        /// Ініціалізує нову кімнату.
        /// </summary>
        public Room(string number, int capacity, decimal price)
        {
            RoomNumber = number;
            Capacity = capacity;
            PricePerNight = price;
        }
    }

    /// <summary>
    /// Представляє заявку на бронювання (замовлення).
    /// Демонструє відношення: Асоціація з Client та Room.
    /// </summary>
    public class Booking
    {
        // Окремий лічильник для бронювань
        private static int _nextBookingId = 1;

        /// <summary>
        /// Унікальний цілочисельний ідентифікатор бронювання.
        /// </summary>
        public int BookingId { get; private set; }
        public Client Guest { get; private set; }
        public Room BookedRoom { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string RequestNote { get; set; }

        public decimal TotalCost
        {
            get
            {
                int days = (EndDate - StartDate).Days;
                return days > 0 ? days * BookedRoom.PricePerNight : BookedRoom.PricePerNight;
            }
        }

        public Booking(Client guest, Room room, DateTime start, DateTime end, string note = "")
        {
            if (end <= start)
                throw new BookingException("Check-out date must be later than check-in date.");

            // Видаємо цілочисельний ID
            BookingId = _nextBookingId++; 
            
            Guest = guest;
            BookedRoom = room;
            StartDate = start;
            EndDate = end;
            RequestNote = note;
        }
    }

    /// <summary>
    /// Представляє готель та управляє його інфраструктурою.
    /// Демонструє Інкапсуляцію, Композицію (Rooms) та Агрегацію (Bookings).
    /// </summary>
    public class Hotel : ISearchable
    {
        private readonly List<Room> _rooms;
        private readonly List<Booking> _bookings;

        /// <summary>
        /// Назва готелю.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Адреса готелю.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Ініціалізує новий готель.
        /// </summary>
        public Hotel(string name, string address)
        {
            Name = name;
            Address = address;
            _rooms = new List<Room>();
            _bookings = new List<Booking>();
        }

        /// <summary>
        /// Додає нову кімнату до готелю.
        /// </summary>
        public void AddRoom(string number, int capacity, decimal price)
        {
            _rooms.Add(new Room(number, capacity, price));
        }

        /// <summary>
        /// Здійснює перевірку та створює нове бронювання.
        /// </summary>
        public Booking BookRoom(Client client, string roomNumber, DateTime start, DateTime end, string note = "")
        {
            var room = _rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (room == null)
                throw new BookingException($"Room {roomNumber} not found in hotel {Name}.");

            bool isOccupied = _bookings.Any(b => b.BookedRoom.RoomNumber == roomNumber &&
                                                 start < b.EndDate && end > b.StartDate);
            if (isOccupied)
                throw new BookingException($"Room {roomNumber} is already booked for these dates.");

            var booking = new Booking(client, room, start, end, note);
            _bookings.Add(booking);
            return booking;
        }

        /// <summary>
        /// Видаляє бронювання за ідентифікатором.
        /// </summary>
        /// <summary>
        /// Видаляє бронювання за цілочисельним ідентифікатором.
        /// </summary>
        public void CancelBooking(int bookingId)
        {
            var booking = _bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking != null)
            {
                _bookings.Remove(booking);
            }
            else
            {
                throw new BookingException($"Booking with ID {bookingId} not found.");
            }
        }

        /// <summary>
        /// Повертає список усіх бронювань у готелі.
        /// </summary>
        public List<Booking> GetAllBookings() => _bookings;

        /// <summary>
        /// Розраховує загальну кількість місць у готелі.
        /// </summary>
        public int GetTotalCapacity() => _rooms.Sum(r => r.Capacity);

        /// <summary>
        /// Перевіряє чи збігається пошуковий запит з назвою або адресою готелю.
        /// </summary>
        public bool Matches(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return false;
            string lowerKeyword = keyword.ToLower();
            return Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || Address.Contains(keyword, StringComparison.OrdinalIgnoreCase);

        }
    }

    /// <summary>
    /// Служба для демонстрації відношення Залежності (Dependency).
    /// </summary>
    public static class SearchService
    {
        /// <summary>
        /// Виконує пошук серед списку об'єктів. Залежить від інтерфейсу ISearchable.
        /// </summary>
        public static IEnumerable<ISearchable> Search(IEnumerable<ISearchable> items, string keyword)
        {
            return items.Where(item => item.Matches(keyword));
        }
    }
}
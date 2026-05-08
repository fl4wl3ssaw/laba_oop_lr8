using System;
using System.Collections.Generic;
using System.Linq;
using laba_oop_lr8;

namespace HotelReservation.UI
{
    class Program
    {
        // Статичні змінні для зберігання стану нашої програми під час її роботи
        static Hotel myHotel;
        static List<Client> clients = new List<Client>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            InitializeData(); // Заповнюємо готель тестовими даними

            bool isRunning = true;

            // Головний цикл програми
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine($"=== Welcome to {myHotel.Name} System ===");
                Console.WriteLine("1. Register a new client");
                Console.WriteLine("2. View all clients");
                Console.WriteLine("3. Book a room");
                Console.WriteLine("4. View all bookings");
                Console.WriteLine("5. Cancel a booking");
                Console.WriteLine("6. Search by keyword"); // <--- ДОДАНО НОВИЙ ПУНКТ
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    // Обробка вибору користувача
                    switch (choice)
                    {
                        case "1":
                            RegisterClient();
                            break;
                        case "2":
                            ViewClients();
                            break;
                        case "3":
                            BookRoomMenu();
                            break;
                        case "4":
                            ViewBookings();
                            break;
                        case "5":
                            CancelBookingMenu();
                            break;
                        case "6":
                            SearchMenu(); // <--- ВИКЛИК НОВОГО МЕТОДУ
                            break;
                        case "0":
                            isRunning = false;
                            Console.WriteLine("Exiting the system. Goodbye!");
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (BookingException ex) // Перехоплюємо помилки нашої бізнес-логіки
                {
                    Console.WriteLine($"\n[BUSINESS LOGIC ERROR]: {ex.Message}");
                }
                catch (Exception ex) // Перехоплюємо системні помилки
                {
                    Console.WriteLine($"\n[SYSTEM ERROR]: {ex.Message}");
                }

                if (isRunning)
                {
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Початкове налаштування готелю та номерного фонду.
        /// </summary>
        static void InitializeData()
        {
            myHotel = new Hotel("Grand Hotel Kyiv", "1 Khreshchatyk St.");
            myHotel.AddRoom("101", 2, 1500m);
            myHotel.AddRoom("102", 1, 1000m);
            myHotel.AddRoom("201", 4, 3000m);
            myHotel.AddRoom("202", 2, 1600m);
            
            // Додамо кількох клієнтів для зручності тестування пошуку
            clients.Add(new Client("Ivan", "Petrenko", "+380501234567"));
            clients.Add(new Client("Maria", "Kovalenko", "+380671234567"));
            clients.Add(new Client("John", "Doe", "+15551234567"));
        }

        /// <summary>
        /// Метод для реєстрації нового клієнта через консоль.
        /// </summary>
        static void RegisterClient()
        {
            Console.WriteLine("--- Client Registration ---");
            Console.Write("Enter First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Enter Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Enter Phone Number: ");
            string phone = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                Console.WriteLine("Name cannot be empty!");
                return;
            }

            Client newClient = new Client(firstName, lastName, phone);
            clients.Add(newClient);
            Console.WriteLine($"\nSuccess! Client {firstName} {lastName} registered with ID: {newClient.Id}");
        }

        /// <summary>
        /// Відображає список усіх клієнтів.
        /// </summary>
        static void ViewClients()
        {
            Console.WriteLine("--- Registered Clients ---");
            if (clients.Count == 0)
            {
                Console.WriteLine("No clients registered yet.");
                return;
            }

            foreach (var client in clients)
            {
                Console.WriteLine(client.GetDescription());
            }
        }

        /// <summary>
        /// Інтерактивне меню для створення бронювання.
        /// </summary>
        static void BookRoomMenu()
        {
            Console.WriteLine("--- Room Booking ---");
            
            if (clients.Count == 0)
            {
                Console.WriteLine("You need to register a client first (Option 1).");
                return;
            }

            Console.Write("Enter Client ID: ");
            if (!int.TryParse(Console.ReadLine(), out int clientId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            Client selectedClient = clients.FirstOrDefault(c => c.Id == clientId);
            if (selectedClient == null)
            {
                Console.WriteLine("Client not found.");
                return;
            }

            Console.Write("Enter Room Number (Available: 101, 102, 201, 202): ");
            string roomNumber = Console.ReadLine();

            Console.Write("Enter Check-in Date (YYYY-MM-DD): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime checkIn))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.Write("Enter Check-out Date (YYYY-MM-DD): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime checkOut))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.Write("Any special requests? (Press Enter to skip): ");
            string note = Console.ReadLine();

            Booking newBooking = myHotel.BookRoom(selectedClient, roomNumber, checkIn, checkOut, note);
            
            Console.WriteLine($"\nSUCCESS! Room {roomNumber} booked for {selectedClient.LastName}.");
            Console.WriteLine($"Booking ID: {newBooking.BookingId}");
            Console.WriteLine($"Total Cost: {newBooking.TotalCost} UAH.");
        }

        /// <summary>
        /// Відображає всі поточні бронювання у готелі.
        /// </summary>
        static void ViewBookings()
        {
            Console.WriteLine("--- All Bookings ---");
            var bookings = myHotel.GetAllBookings();

            if (bookings.Count == 0)
            {
                Console.WriteLine("No active bookings.");
                return;
            }

            foreach (var b in bookings)
            {
                Console.WriteLine($"[Booking ID: {b.BookingId}] Room: {b.BookedRoom.RoomNumber} | " +
                                  $"Guest: {b.Guest.LastName} | " +
                                  $"Dates: {b.StartDate.ToShortDateString()} -> {b.EndDate.ToShortDateString()}");
            }
        }

        /// <summary>
        /// Метод для скасування бронювання користувачем.
        /// </summary>
        static void CancelBookingMenu()
        {
            Console.WriteLine("--- Cancel Booking ---");
            Console.Write("Enter Booking ID to cancel: ");
            
            if (int.TryParse(Console.ReadLine(), out int bookingId))
            {
                myHotel.CancelBooking(bookingId);
                Console.WriteLine($"Booking {bookingId} has been successfully canceled.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        /// <summary>
        /// Метод для універсального пошуку по всій системі.
        /// Демонструє поліморфізм та використання інтерфейсу ISearchable.
        /// </summary>
        static void SearchMenu()
        {
            Console.WriteLine("--- Global Search ---");
            Console.Write("Enter keyword to search (name, phone, hotel name or address): ");
            string keyword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                Console.WriteLine("Keyword cannot be empty.");
                return;
            }

            // 1. Створюємо "базу" для пошуку, яка приймає БУДЬ-ЯКІ об'єкти, що реалізують ISearchable
            List<ISearchable> searchDatabase = new List<ISearchable>();
            
            // 2. Додаємо туди наш готель (він підтримує пошук за назвою та адресою)
            searchDatabase.Add(myHotel); 
            
            // 3. Додаємо туди всіх клієнтів (вони підтримують пошук за ім'ям, прізвищем та телефоном)
            searchDatabase.AddRange(clients); 

            // 4. Передаємо цю збірну солянку до нашого універсального сервісу пошуку
            var results = SearchService.Search(searchDatabase, keyword).ToList();

            Console.WriteLine($"\nFound {results.Count} result(s):");
            
            // 5. Виводимо результати, перевіряючи, який саме об'єкт ми знайшли
            foreach (var item in results)
            {
                if (item is Hotel h)
                {
                    Console.WriteLine($"[HOTEL MATCH] {h.Name}, Address: {h.Address}");
                }
                else if (item is Client c)
                {
                    Console.WriteLine($"[CLIENT MATCH] {c.GetDescription()}");
                }
            }
        }
    }
}
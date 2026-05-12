using System;
using laba_oop_lr8;

namespace HotelReservation.UI
{
    class Program
    {
        // Єдина точка доступу до даних та бізнес-логіки
        static InstanceManager manager = new InstanceManager();

        static void Main(string[] args)
        {
            // Встановлюємо кодування для коректного відображення символів
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool isRunning = true;

            // Головний цикл програми
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== Hotel Management System ===");
                Console.WriteLine("1. Manage Clients (Add/Edit/Delete/View/Sort)");
                Console.WriteLine("2. Manage Hotel & Bookings (Stats/Dates/Notes/View Hotels)");
                Console.WriteLine("3. Book a room in a hotel");
                Console.WriteLine("4. Global Search");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    // Обробка вибору головного меню
                    switch (choice)
                    {
                        case "1":
                            ClientMenu();
                            break;
                        case "2":
                            HotelAndBookingMenu();
                            break;
                        case "3":
                            BookRoomFlow();
                            break;
                        case "4":
                            PerformSearch();
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
                catch (BookingException ex) // Перехоплюємо помилки бізнес-логіки
                {
                    Console.WriteLine($"\n[BUSINESS LOGIC ERROR]: {ex.Message}");
                }
                catch (FormatException) // Перехоплюємо помилки неправильного вводу (наприклад, букви замість цифр)
                {
                    Console.WriteLine("\n[INPUT ERROR]: Invalid format. Please enter correct data types (e.g., numbers for ID).");
                }
                catch (Exception ex) // Перехоплюємо інші системні помилки
                {
                    Console.WriteLine($"\n[SYSTEM ERROR]: {ex.Message}");
                }

                if (isRunning)
                {
                    Console.WriteLine("\nPress any key to return to the main menu...");
                    Console.ReadKey();
                }
            }
        }
        
        static void ClientMenu()
        {
            Console.WriteLine("--- Client Management ---");
            Console.WriteLine("1. Add Client (2.1)");
            Console.WriteLine("2. View All Clients (2.5)");
            Console.WriteLine("3. View Client by ID (2.4)");
            Console.WriteLine("4. Edit Client (2.3)");
            Console.WriteLine("5. Delete Client (2.2)");
            Console.WriteLine("6. Sort by First Name (2.6)");
            Console.WriteLine("7. Sort by Last Name (2.7)");
            Console.Write("Select option: ");
            
            string opt = Console.ReadLine();
            Console.WriteLine();

            switch (opt)
            {
                case "1":
                    Console.Write("First Name: "); string fName = Console.ReadLine();
                    Console.Write("Last Name: "); string lName = Console.ReadLine();
                    Console.Write("Phone: "); string phone = Console.ReadLine();
                    var c = manager.AddClient(fName, lName, phone);
                    Console.WriteLine($"Success! Client added with ID: {c.Id}");
                    break;
                case "2":
                    var allClients = manager.GetAllClients();
                    if (allClients.Count == 0) Console.WriteLine("No clients found.");
                    else allClients.ForEach(client => Console.WriteLine(client.GetDescription()));
                    break;
                case "3":
                    Console.Write("Enter Client ID: ");
                    int id = int.Parse(Console.ReadLine());
                    var found = manager.GetClientById(id);
                    Console.WriteLine(found != null ? found.GetDescription() : "Client not found.");
                    break;
                case "4":
                    Console.Write("Enter Client ID to edit: ");
                    int editId = int.Parse(Console.ReadLine());
                    Console.Write("New First Name (or press Enter to skip): "); string nFirst = Console.ReadLine();
                    Console.Write("New Last Name (or press Enter to skip): "); string nLast = Console.ReadLine();
                    Console.Write("New Phone (or press Enter to skip): "); string nPhone = Console.ReadLine();
                    if (manager.UpdateClient(editId, nFirst, nLast, nPhone)) 
                        Console.WriteLine("Client updated successfully.");
                    else 
                        Console.WriteLine("Client not found.");
                    break;
                case "5":
                    Console.Write("Enter Client ID to delete: ");
                    int delId = int.Parse(Console.ReadLine());
                    if (manager.RemoveClient(delId)) 
                        Console.WriteLine("Client deleted successfully.");
                    else 
                        Console.WriteLine("Client not found.");
                    break;
                case "6":
                    manager.GetClientsSortedByFirstName().ForEach(client => Console.WriteLine(client.GetDescription()));
                    break;
                case "7":
                    manager.GetClientsSortedByLastName().ForEach(client => Console.WriteLine(client.GetDescription()));
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
        
        static void HotelAndBookingMenu()
        {
            Console.WriteLine("--- Hotels Management ---");
            Console.WriteLine("1. Add a New Hotel (1.1)");
            Console.WriteLine("2. Delete a Hotel (1.2)");
            Console.WriteLine("3. View All Hotels (1.4)");
            Console.WriteLine("4. Manage Specific Hotel (Bookings & Stats)");
            Console.Write("Select option: ");
            
            string opt = Console.ReadLine();
            Console.WriteLine();

            switch (opt)
            {
                case "1":
                    Console.Write("Enter Hotel Name: "); 
                    string hName = Console.ReadLine();
                    Console.Write("Enter Hotel Address: "); 
                    string hAddress = Console.ReadLine();
                    
                    manager.AddHotel(hName, hAddress);
                    Console.WriteLine($"Success! Hotel '{hName}' added to the system.");
                    break;

                case "2":
                    Console.Write("Enter Hotel Name to delete: "); 
                    string delName = Console.ReadLine();
                    
                    if (manager.RemoveHotel(delName))
                        Console.WriteLine($"Hotel '{delName}' has been successfully deleted.");
                    else
                        Console.WriteLine("Hotel not found.");
                    break;

                case "3":
                    var hotels = manager.GetAllHotels();
                    if (hotels.Count == 0) Console.WriteLine("No hotels available in the system.");
                    else
                    {
                        foreach (var h in hotels)
                            Console.WriteLine($"- {h.Name} ({h.Address}). Total beds: {h.GetTotalCapacity()}");
                    }
                    break;

                case "4":
                    ManageSpecificHotelFlow();
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
        
        static void ManageSpecificHotelFlow()
        {
            Console.Write("Enter the exact Hotel Name you want to manage (e.g., 'Grand Hotel Kyiv'): ");
            string hotelName = Console.ReadLine();
            
            var hotel = manager.GetHotelByName(hotelName);
            if (hotel == null)
            {
                Console.WriteLine("Hotel not found in the system.");
                return;
            }

            Console.WriteLine($"\n--- Managing Hotel: {hotel.Name} ---");
            Console.WriteLine("1. View Bookings by Date Range (1.8)");
            Console.WriteLine("2. Change Booking Note (1.7)");
            Console.WriteLine("3. View Specific Booking Details (3.3)");
            Console.WriteLine("4. Room Occupancy Stats for Date (3.4 & 3.5)");
            Console.WriteLine("5. View All Current Guests (3.7)");
            Console.WriteLine("6. Cancel a Booking (3.2)");
            Console.Write("Select option: ");
            
            string opt = Console.ReadLine();
            Console.WriteLine();

            switch (opt)
            {
                case "1":
                    Console.Write("Start Date (YYYY-MM-DD): "); DateTime sd = DateTime.Parse(Console.ReadLine());
                    Console.Write("End Date (YYYY-MM-DD): "); DateTime ed = DateTime.Parse(Console.ReadLine());
                    var bookings = hotel.GetBookingsByDateRange(sd, ed);
                    if (bookings.Count == 0) Console.WriteLine("No bookings found in this range.");
                    foreach (var b in bookings) 
                        Console.WriteLine($"ID: {b.BookingId}, Room: {b.BookedRoom.RoomNumber}, Guest: {b.Guest.LastName}");
                    break;
                case "2":
                    Console.Write("Booking ID: "); int bIdNote = int.Parse(Console.ReadLine());
                    Console.Write("New Note: "); string newNote = Console.ReadLine();
                    hotel.UpdateBookingNote(bIdNote, newNote);
                    Console.WriteLine("Note updated successfully.");
                    break;
                case "3":
                    Console.Write("Booking ID: "); int bId = int.Parse(Console.ReadLine());
                    var booking = hotel.GetBookingById(bId);
                    if (booking != null) 
                        Console.WriteLine($"Room: {booking.BookedRoom.RoomNumber}, Guest: {booking.Guest.GetDescription()}, Cost: {booking.TotalCost} UAH, Note: {booking.RequestNote}");
                    else 
                        Console.WriteLine("Booking not found.");
                    break;
                case "4":
                    Console.Write("Enter Date to check (YYYY-MM-DD): "); DateTime targetDate = DateTime.Parse(Console.ReadLine());
                    
                    var occupied = hotel.GetOccupiedRooms(targetDate);
                    Console.WriteLine($"\n--- Occupied Rooms ({occupied.Count}) ---");
                    int occBeds = 0;
                    foreach (var r in occupied) { Console.WriteLine($"- Room: {r.RoomNumber} ({r.Capacity} beds)"); occBeds += r.Capacity; }
                    Console.WriteLine($"Total Occupied Beds: {occBeds}");

                    var free = hotel.GetFreeRooms(targetDate);
                    Console.WriteLine($"\n--- Free Rooms ({free.Count}) ---");
                    int freeBeds = 0;
                    foreach (var r in free) { Console.WriteLine($"- Room: {r.RoomNumber} ({r.Capacity} beds)"); freeBeds += r.Capacity; }
                    Console.WriteLine($"Total Free Beds: {freeBeds}");
                    break;
                case "5":
                    var guests = hotel.GetClientsWithBookings();
                    if (guests.Count == 0) Console.WriteLine("No active guests found.");
                    guests.ForEach(g => Console.WriteLine(g.GetDescription()));
                    break;
                case "6":
                    Console.Write("Enter Booking ID to cancel: ");
                    int cancelId = int.Parse(Console.ReadLine());
                    hotel.CancelBooking(cancelId);
                    Console.WriteLine($"Booking {cancelId} has been successfully canceled.");
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

       
        static void BookRoomFlow()
        {
            Console.WriteLine("--- Room Booking ---");
            Console.Write("Enter Client ID: ");
            int clientId = int.Parse(Console.ReadLine());
            var client = manager.GetClientById(clientId);
            
            if (client == null) throw new BookingException("Client not found in the system.");

            Console.Write("Enter Hotel Name (e.g., 'Grand Hotel Kyiv'): ");
            string hotelName = Console.ReadLine();
            var hotel = manager.GetHotelByName(hotelName);

            if (hotel == null) throw new BookingException("Hotel not found.");

            Console.Write("Enter Room Number: ");
            string roomNum = Console.ReadLine();

            Console.Write("Check-in Date (YYYY-MM-DD): ");
            DateTime inDate = DateTime.Parse(Console.ReadLine());

            Console.Write("Check-out Date (YYYY-MM-DD): ");
            DateTime outDate = DateTime.Parse(Console.ReadLine());

            Console.Write("Any special requests? (Press Enter to skip): ");
            string note = Console.ReadLine();
            
            var booking = hotel.BookRoom(client, roomNum, inDate, outDate, note);
            Console.WriteLine($"\nSUCCESS! Room {roomNum} booked for {client.LastName}.");
            Console.WriteLine($"Booking ID: {booking.BookingId}");
            Console.WriteLine($"Total Cost: {booking.TotalCost} UAH");
        }

      
        static void PerformSearch()
        {
            Console.WriteLine("--- Global Search ---");
            Console.Write("Enter keyword (name, phone, address): ");
            string keyword = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(keyword))
            {
                Console.WriteLine("Keyword cannot be empty.");
                return;
            }

            var results = manager.GlobalSearch(keyword);
            
            int count = 0;
            foreach (var item in results)
            {
                count++;
                if (item is Hotel h) Console.WriteLine($"[HOTEL] {h.Name}, Address: {h.Address}");
                else if (item is Client c) Console.WriteLine($"[CLIENT] {c.GetDescription()}");
            }

            Console.WriteLine($"\nFound {count} result(s).");
        }
    }
}
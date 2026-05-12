using System;
using System.Collections.Generic;
using System.Linq;

namespace laba_oop_lr8
{
    /// <summary>
    /// Клас-менеджер для централізованого управління всією системою (готелями та клієнтами).
    /// </summary>
    public class InstanceManager
    {
        private List<Hotel> _hotels;
        private List<Client> _clients;

        public InstanceManager()
        {
            _hotels = new List<Hotel>();
            _clients = new List<Client>();
            InitializeTestData();
        }
        
        // 1.1 Додавати готель
        public void AddHotel(string name, string address)
        {
            _hotels.Add(new Hotel(name, address));
        }

        // 1.2 Видаляти готель
        public bool RemoveHotel(string name)
        {
            var hotel = _hotels.FirstOrDefault(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (hotel != null)
            {
                _hotels.Remove(hotel);
                return true;
            }
            return false;
        }

        // 1.4 Перегляд всіх готелів
        public List<Hotel> GetAllHotels()
        {
            return _hotels;
        }

        // Отримати конкретний готель за назвою
        public Hotel GetHotelByName(string name)
        {
            return _hotels.FirstOrDefault(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        
        // 2.1 Добавлення клієнтів
        public Client AddClient(string firstName, string lastName, string phone)
        {
            var client = new Client(firstName, lastName, phone);
            _clients.Add(client);
            return client;
        }

        // 2.2 Видалення клієнтів
        public bool RemoveClient(int clientId)
        {
            var client = _clients.FirstOrDefault(c => c.Id == clientId);
            if (client != null)
            {
                _clients.Remove(client);
                return true;
            }
            return false;
        }

        // 2.3 Змінення даних про клієнтів
        public bool UpdateClient(int clientId, string newFirstName, string newLastName, string newPhone)
        {
            var client = _clients.FirstOrDefault(c => c.Id == clientId);
            if (client != null)
            {
                if (!string.IsNullOrWhiteSpace(newFirstName)) client.FirstName = newFirstName;
                if (!string.IsNullOrWhiteSpace(newLastName)) client.LastName = newLastName;
                if (!string.IsNullOrWhiteSpace(newPhone)) client.PhoneNumber = newPhone;
                return true;
            }
            return false;
        }

        // 2.4 Переглянути дані про конкретного клієнта
        public Client GetClientById(int id)
        {
            return _clients.FirstOrDefault(c => c.Id == id);
        }

        // 2.5 Переглянути дані про всіх клієнтів
        public List<Client> GetAllClients()
        {
            return _clients;
        }

        // 2.6 Відсортувати список по імені
        public List<Client> GetClientsSortedByFirstName()
        {
            return _clients.OrderBy(c => c.FirstName).ToList();
        }

        // 2.7 Відсортувати список по прізвищу
        public List<Client> GetClientsSortedByLastName()
        {
            return _clients.OrderBy(c => c.LastName).ToList();
        }
        
        public IEnumerable<ISearchable> GlobalSearch(string keyword)
        {
            List<ISearchable> searchDatabase = new List<ISearchable>();
            searchDatabase.AddRange(_hotels);
            searchDatabase.AddRange(_clients);
            
            return SearchService.Search(searchDatabase, keyword);
        }

        // Початкове наповнення даними
        private void InitializeTestData()
        {
            AddHotel("Grand Hotel Kyiv", "1 Khreshchatyk St.");
            AddHotel("Lviv Central", "10 Rynok Sq.");

            // Додаємо кімнати до першого готелю
            var kievHotel = GetHotelByName("Grand Hotel Kyiv");
            kievHotel.AddRoom("101", 2, 1500m);
            kievHotel.AddRoom("102", 1, 1000m);

            AddClient("Ivan", "Petrenko", "+380501234567");
            AddClient("Maria", "Kovalenko", "+380671234567");
            AddClient("John", "Doe", "+15551234567");
        }
    }
}
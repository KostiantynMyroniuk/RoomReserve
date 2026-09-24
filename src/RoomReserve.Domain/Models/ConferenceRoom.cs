using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Domain.Models
{
    public class ConferenceRoom
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = default!;
        public int Capacity { get; private set; }
        public decimal PricePerHour { get; private set; }

        private readonly List<Service> _services = [];
        public IReadOnlyList<Service> Services => _services.AsReadOnly();

        private readonly List<Booking> _bookings = [];
        public IReadOnlyList<Booking> Bookings => _bookings.AsReadOnly();

        private ConferenceRoom()
        {
        }

        public static ConferenceRoom Create(
            string name,
            int capacity,
            decimal pricePerHour)
        {
            return new ConferenceRoom
            {
                Id = Guid.CreateVersion7(),
                Name = name,
                Capacity = capacity,
                PricePerHour = pricePerHour
            };
        }

        public void AddService(string name, decimal price)
        {
            var service = Service.Create(name, price);
            _services.Add(service);
        }

        public void UpdateRoomDetails(string name, int capacity, decimal pricePerHour)
        {
            Name = name;
            Capacity = capacity;
            PricePerHour = pricePerHour;
        }

        public Booking BookRoom(DateTime startTime, DateTime endTime, List<Service> selectedServices)
        {
            foreach (var service in selectedServices)
            {
                if (_services.Any(s => s.Name == service.Name))
                {
                    throw new InvalidOperationException($"Service '{service.Name}' is not available for this room.");
                }
            }

            var booking = Booking.Create(this.Id, startTime, endTime, selectedServices);
            _bookings.Add(booking);
            return booking;
        }
    }
}

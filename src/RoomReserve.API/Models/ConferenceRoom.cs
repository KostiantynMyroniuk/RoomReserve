using RoomReserve.API.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RoomReserve.API.Models
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

        [Timestamp]
        public byte[] RowVersion { get; private set; } = default!;

        private ConferenceRoom()
        {
        }

        public static ConferenceRoom Create(
            string name,
            int capacity,
            decimal pricePerHour)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Room name must not be empty.");

            if (capacity <= 0)
                throw new DomainException("Room capacity must be greater than zero.");

            if (pricePerHour < 0)
                throw new DomainException("Price per hour cannot be negative.");

            return new ConferenceRoom
            {
                Id = Guid.CreateVersion7(),
                Name = name.Trim(),
                Capacity = capacity,
                PricePerHour = pricePerHour
            };
        }

        public void UpdateRoomDetails(string name, int capacity, decimal pricePerHour, IEnumerable<Service> services)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Room name must not be empty.");

            if (capacity <= 0)
                throw new DomainException("Room capacity must be greater than zero.");

            if (pricePerHour < 0)
                throw new DomainException("Price per hour cannot be negative.");

            Name = name;
            Capacity = capacity;
            PricePerHour = pricePerHour;

            _services.Clear();

            if (services != null)
            {
                _services.AddRange(services);
            }
        }
    }
}

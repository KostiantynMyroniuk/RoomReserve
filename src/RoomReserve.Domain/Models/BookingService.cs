using RoomReserve.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Domain.Models
{
    public class BookingService
    {
        public Guid BookingId { get; private set; }
        public Booking Booking { get; private set; } = default!;

        public Guid ServiceId { get; private set; }
        public Service Service { get; private set; } = default!;

        public string ServiceName { get; private set; } = default!;
        public decimal Price { get; private set; }

        private BookingService() { }

        internal static BookingService Create(
            Guid bookingId,
            Guid serviceId,
            string serviceName,
            decimal price)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new DomainException("Service name cannot be empty.");

            if (price < 0)
                throw new DomainException("Service price cannot be negative.");

            return new BookingService
            {
                BookingId = bookingId,
                ServiceId = serviceId,
                ServiceName = serviceName,
                Price = price
            };
        }
    }
}

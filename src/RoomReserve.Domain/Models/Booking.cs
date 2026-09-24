using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Domain.Models
{
    public class Booking
    {
        public Guid Id { get; private set; }
        public Guid RoomId { get; private set; }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public decimal TotalPrice { get; private set; }

        public IReadOnlyList<Service> Services { get; private set; } = [];

        public static Booking Create(
            Guid roomId,
            DateTime startTime,
            DateTime endTime,
            List<Service> selectedServices)
        {
            return new Booking
            {
                Id = Guid.CreateVersion7(),
                RoomId = roomId,
                StartTime = startTime,
                EndTime = endTime,
                Services = selectedServices
            };
        }
    }
}

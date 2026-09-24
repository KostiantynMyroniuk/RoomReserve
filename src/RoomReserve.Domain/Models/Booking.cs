using RoomReserve.Domain.Exceptions;
using RoomReserve.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Domain.Models
{
    public class Booking
    {
        public Guid Id { get; private set; }
        public Guid RoomId { get; private set; }
        public string BookedByUserId { get; private set; } = default!;

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public decimal RoomPricePerHour { get; private set; }

        public decimal TotalPrice { get; private set; }

        private readonly List<BookingService> _bookingServices = [];
        public IReadOnlyList<BookingService> BookingServices => _bookingServices.AsReadOnly();

        public static Booking Create(
            Guid roomId,
            string bookedByUserId,
            DateTime startTime,
            DateTime endTime,
            decimal roomPricePerHour,
            IReadOnlyCollection<Service> selectedServices)
        {
            if (string.IsNullOrWhiteSpace(bookedByUserId))
            {
                throw new DomainException("A booking must be associated with a user.");
            }

            if (startTime >= endTime)
            {
                throw new DomainException("Start time must be before end time.");
            }

            if (startTime < DateTime.UtcNow)
            {
                throw new DomainException("Cannot book a room in the past.");
            }

            var booking = new Booking
            {
                Id = Guid.CreateVersion7(),
                RoomId = roomId,
                BookedByUserId = bookedByUserId,
                StartTime = startTime,
                EndTime = endTime,
                RoomPricePerHour = roomPricePerHour
            };

            foreach (var service in selectedServices ?? [])
            {
                booking._bookingServices.Add(BookingService.Create(
                    booking.Id,
                    service.Id,
                    service.Name,
                    service.Price));
            }

            booking.RecalculateTotalPrice();

            return booking;
        }

        public void AddService(
            Guid serviceId,
            string serviceName,
            decimal price)
        {
            if (_bookingServices.Any(x => x.ServiceId == serviceId))
                throw new DomainException(
                    $"Service '{serviceName}' is already added to this booking.");


            _bookingServices.Add(BookingService.Create(
                this.Id, 
                serviceId,
                serviceName,
                price));

            RecalculateTotalPrice();
        }

        public void RemoveService(Guid serviceId)
        {
            var bookingService = _bookingServices.FirstOrDefault(s => s.ServiceId == serviceId)
                ?? throw new ServiceNotFoundException($"Service with ID {serviceId} is not part of this booking.");

            _bookingServices.Remove(bookingService);
            RecalculateTotalPrice();
        }

        public bool OverlapsWith(DateTime otherStart, DateTime otherEnd)
        {
            return StartTime < otherEnd && otherStart < EndTime;
        }

        private void RecalculateTotalPrice()
        {
            var roomCost = PricingCalculator.CalculateRoomCost(StartTime, EndTime, RoomPricePerHour);
            var servicesCost = _bookingServices.Sum(s => s.Price);
            TotalPrice = roomCost + servicesCost;
        }
    }
}

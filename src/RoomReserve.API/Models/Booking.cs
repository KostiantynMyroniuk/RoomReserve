using RoomReserve.API.Models.Exceptions;
using RoomReserve.API.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.API.Models
{
    public class Booking
    {
        public Guid Id { get; private set; }
        public Guid RoomId { get; private set; }

        public DateOnly Date { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }

        public decimal RoomPricePerHour { get; private set; }

        public decimal TotalPrice { get; private set; }

        private readonly List<BookingService> _bookingServices = [];
        public IReadOnlyList<BookingService> BookingServices => _bookingServices.AsReadOnly();

        public static Booking Create(
            Guid roomId,
            DateOnly date,
            TimeOnly startTime,
            TimeOnly endTime,
            decimal roomPricePerHour,
            IEnumerable<Service> selectedServices)
        {

            if (startTime >= endTime)
            {
                throw new DomainException("Start time must be before end time.");
            }

            var booking = new Booking
            {
                Id = Guid.CreateVersion7(),
                RoomId = roomId,
                Date = date,
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

        private void RecalculateTotalPrice()
        {
            var roomCost = PricingCalculator.CalculateRoomCost(StartTime, EndTime, RoomPricePerHour);
            var servicesCost = _bookingServices.Sum(s => s.Price);
            TotalPrice = roomCost + servicesCost;
        }
    }
}

using RoomReserve.API.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.API.Services
{
    public static class PricingCalculator
    {
        private static readonly (TimeOnly Start, TimeOnly End, decimal Multiplier)[] RateSegments =
        [
            (new TimeOnly(6, 0),  new TimeOnly(9, 0),  0.90m), // -10%
            (new TimeOnly(9, 0),  new TimeOnly(12, 0), 1.00m), // standart before peaks
            (new TimeOnly(12, 0), new TimeOnly(14, 0), 1.15m), // +15%
            (new TimeOnly(14, 0), new TimeOnly(18, 0), 1.00m), // standart after peaks
            (new TimeOnly(18, 0), new TimeOnly(23, 0), 0.80m), // -20%
        ];

        public static decimal CalculateRoomCost(TimeOnly startTime, TimeOnly endTime, decimal pricePerHour)
        {
            if (startTime >= endTime)
            {
                throw new DomainException("Start time must be before end time.");
            }

            decimal totalCost = 0m;
            decimal coveredHours = 0m;

            foreach (var (segmentStart, segmentEnd, multiplier) in RateSegments)
            {
                var overlapStart = Max(startTime, segmentStart);
                var overlapEnd = Min(endTime, segmentEnd);

                if (overlapEnd <= overlapStart)
                {
                    continue;
                }

                var hours = (decimal)(overlapEnd - overlapStart).TotalHours;
                coveredHours += hours;
                totalCost += hours * multiplier * pricePerHour;
            }

            var requestedHours = (decimal)(endTime - startTime).TotalHours;

            if (coveredHours < requestedHours - 0.001m)
            {
                throw new DomainException("Booking time must fall within business hours (06:00–23:00).");
            }

            return Math.Round(totalCost, 2);
        }

        private static TimeOnly Max(TimeOnly a, TimeOnly b) => a > b ? a : b;
        private static TimeOnly Min(TimeOnly a, TimeOnly b) => a < b ? a : b;
    }
}

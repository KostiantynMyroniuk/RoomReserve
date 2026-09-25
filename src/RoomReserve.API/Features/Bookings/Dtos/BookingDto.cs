using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.API.Features.Bookings.Dtos
{
    public record BookingDto(
        Guid Id, 
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        decimal TotalPrice);
}

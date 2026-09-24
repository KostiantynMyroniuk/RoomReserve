using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Application.BusinessLogic.Bookings.Dtos
{
    public record BookingDto(
        Guid Id, 
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        decimal TotalPrice);
}

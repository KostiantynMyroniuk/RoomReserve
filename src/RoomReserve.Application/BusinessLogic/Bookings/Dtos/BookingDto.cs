using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Application.BusinessLogic.Bookings.Dtos
{
    public record BookingDto(Guid Id, DateTime StartTime, DateTime EndTime, decimal TotalPrice);
}

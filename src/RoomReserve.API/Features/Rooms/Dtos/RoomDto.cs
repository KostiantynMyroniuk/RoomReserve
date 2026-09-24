using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Application.BusinessLogic.Rooms.Dtos
{
    public record RoomDto(
        Guid Id,
        string Name,
        int Capacity,
        decimal PricePerHour);
}

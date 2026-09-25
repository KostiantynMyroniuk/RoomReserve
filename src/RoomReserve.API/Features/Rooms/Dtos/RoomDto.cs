using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.API.Features.Rooms.Dtos
{
    public record RoomDto(
        Guid Id,
        string Name,
        int Capacity,
        decimal PricePerHour,
        List<RoomServiceDto> Services);
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.API.Features.Rooms.Dtos
{
    public record RoomServiceDto(
        Guid ServiceId,
        string ServiceName, 
        decimal ServicePrice);
}

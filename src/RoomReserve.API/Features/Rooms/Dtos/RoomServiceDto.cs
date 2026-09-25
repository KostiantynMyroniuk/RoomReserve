using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Application.BusinessLogic.Rooms.Dtos
{
    public record RoomServiceDto(
        Guid serviceId,
        string serviceName, 
        decimal servicePrice);
}

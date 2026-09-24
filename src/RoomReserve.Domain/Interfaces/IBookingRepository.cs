using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<Guid> CreateBookingAsync(Guid roomId, DateTime startTime, DateTime endTime, decimal TotalPrice, CancellationToken cancellationToken = default);
    }
}

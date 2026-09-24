using RoomReserve.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Domain.Interfaces
{
    public interface IConferenceRoomRepository
    {
        Task<Guid> CreateRoomAsync(string name, int capacity, decimal pricePerHour, CancellationToken cancellationToken = default);

        Task<IEnumerable<ConferenceRoom>> GetAvailableRoomsFilteredAsync(
            int pageNumber, int pageSize,
            DateOnly? date, TimeOnly? startTime, TimeOnly? endTime, int? capacity, CancellationToken cancellationToken = default);

        Task<ConferenceRoom> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
        Task<ConferenceRoom> UpdateRoomAsync(Guid roomId, string name, int capacity, decimal pricePerHour, CancellationToken cancellationToken = default);
        Task DeleteRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
    }
}

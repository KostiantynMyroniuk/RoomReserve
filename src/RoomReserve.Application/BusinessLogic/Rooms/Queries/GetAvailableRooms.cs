using MediatR;
using RoomReserve.Application.BusinessLogic.Rooms.Dtos;
using RoomReserve.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Application.BusinessLogic.Rooms.Queries
{
    public record GetAvailableRoomsQuery(
        int PageNumber,
        int PageSize,
        DateOnly? Date,
        TimeOnly? StartTime,
        TimeOnly? EndTime,
        int? Capacity) : IRequest<List<RoomDto>>;

    public class GetAvailableRooms(
        IConferenceRoomRepository roomRepository) : IRequestHandler<GetAvailableRoomsQuery, List<RoomDto>>
    {
        public async Task<List<RoomDto>> Handle(GetAvailableRoomsQuery request, CancellationToken cancellationToken)
        {
            var availableRooms = await roomRepository.GetAvailableRoomsFilteredAsync(
                request.PageNumber,
                request.PageSize,
                request.Date,
                request.StartTime,
                request.EndTime,
                request.Capacity, cancellationToken);

            return availableRooms.Select(r => new RoomDto(
                r.Id,
                r.Name,
                r.Capacity,
                r.PricePerHour
            )).ToList();
        }
    }
}

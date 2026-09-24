using MediatR;
using RoomReserve.Application.BusinessLogic.Rooms.Dtos;
using RoomReserve.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Application.BusinessLogic.Rooms.Commands
{
    public record UpdateConferenceRoomCommand(
        Guid RoomId,
        string Name,
        int Capacity,
        decimal PricePerHour) : IRequest<RoomDto>;

    public class UpdateConferenceRoom(
        IConferenceRoomRepository roomRepository) : IRequestHandler<UpdateConferenceRoomCommand, RoomDto>
    {
        public async Task<RoomDto> Handle(UpdateConferenceRoomCommand request, CancellationToken cancellationToken)
        {
            var result = await roomRepository.UpdateRoomAsync(
                request.RoomId,
                request.Name,
                request.Capacity,
                request.PricePerHour);

            return new RoomDto(
                result.Id,
                result.Name,
                result.Capacity,
                result.PricePerHour);
        }
    }
}

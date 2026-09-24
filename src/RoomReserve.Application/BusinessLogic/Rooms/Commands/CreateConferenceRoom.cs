using MediatR;
using RoomReserve.Application.BusinessLogic.Rooms.Dtos;
using RoomReserve.Domain.Interfaces;
using RoomReserve.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Application.BusinessLogic.Rooms.Commands
{
    public record CreateConferenceRoomCommand(
        string Name,
        int Capacity,
        decimal PricePerHour,
        List<ServiceDto> Services) : IRequest<Guid>;

    public class CreateConferenceRoom(
        IConferenceRoomRepository roomRepository) : IRequestHandler<CreateConferenceRoomCommand, Guid>
    {
        public async Task<Guid> Handle(CreateConferenceRoomCommand request, CancellationToken cancellationToken)
        {
            var result = await roomRepository.CreateRoomAsync(
                request.Name,
                request.Capacity,
                request.PricePerHour);

            return result;
        }
    }
}

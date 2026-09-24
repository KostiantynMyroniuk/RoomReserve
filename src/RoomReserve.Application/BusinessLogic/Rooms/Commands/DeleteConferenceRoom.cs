using MediatR;
using RoomReserve.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Application.BusinessLogic.Rooms.Commands
{
    public record DeleteConferenceRoomCommand(Guid RoomId) : IRequest;

    public class DeleteConferenceRoom(
        IConferenceRoomRepository roomRepository) : IRequestHandler<DeleteConferenceRoomCommand>
    {
        public async Task Handle(DeleteConferenceRoomCommand request, CancellationToken cancellationToken)
        {
            await roomRepository.DeleteRoomAsync(request.RoomId);
        }
    }
}

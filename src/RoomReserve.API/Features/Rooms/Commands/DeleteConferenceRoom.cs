using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReserve.API.Infrastructure.Persistence;
using RoomReserve.API.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Application.BusinessLogic.Rooms.Commands
{
    public record DeleteConferenceRoomCommand(Guid RoomId) : IRequest<Result>;

    public class DeleteConferenceRoomCommandHandler(
        ApplicationDbContext context) : IRequestHandler<DeleteConferenceRoomCommand, Result>
    {
        public async Task<Result> Handle(DeleteConferenceRoomCommand request, CancellationToken cancellationToken)
        {
            var rowsAffected = await context.ConferenceRooms
                .Where(r => r.Id == request.RoomId)
                .ExecuteDeleteAsync(cancellationToken);

            if (rowsAffected == 0)
                return Result.Failure(ResultError.NotFound($"Conference room {request.RoomId} not found."));

            return Result.Success();
        }
    }

    public class DeleteConferenceRoomEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/rooms/{roomId:guid}", async (
                [FromRoute] Guid roomId,
                ISender sender) =>
            {
                var result = await sender.Send(new DeleteConferenceRoomCommand(roomId));

                if (result.IsSuccess)
                {
                    return Results.NoContent();
                }

                return Results.NotFound(result.Error?.Message);
            })
            .WithName("DeleteRoom")
            .WithTags("Rooms");
        }
    }
}

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReserve.API.Infrastructure.Persistence;
using RoomReserve.API.Models.Common;
using RoomReserve.API.Features.Rooms.Dtos;

namespace RoomReserve.API.Features.Rooms.Commands
{
    public record UpdateConferenceRoomCommand(
        Guid RoomId,
        string Name,
        int Capacity,
        decimal PricePerHour,
        List<Guid> ServiceIds) : IRequest<Result<RoomDto>>;

    public class UpdateConferenceRoomCommandHandler(
        ApplicationDbContext context,
        ILogger<UpdateConferenceRoomCommandHandler> logger) : IRequestHandler<UpdateConferenceRoomCommand, Result<RoomDto>>
    {
        public async Task<Result<RoomDto>> Handle(UpdateConferenceRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await context.ConferenceRooms
                .Include(r => r.Services)
                .FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);

            if (room == null)
            {
                logger.LogWarning("Conference room not found: {RoomId}", request.RoomId);
                return Result<RoomDto>.Failure(ResultError.NotFound($"Conference room {request.RoomId} not found."));
            }

            var services = await context.Services
                .Where(s => request.ServiceIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            var missingServices = request.ServiceIds
                .Except(services.Select(s => s.Id)).ToList();

            if (missingServices.Any())
            {
                logger.LogWarning("Some services are missing: {MissingServices}", missingServices);
                return Result<RoomDto>.Failure(ResultError.BadRequest($"Some services are missing: {string.Join(", ", missingServices)}"));
            }

            room.UpdateRoomDetails(request.Name, request.Capacity, request.PricePerHour, services);

            await context.SaveChangesAsync(cancellationToken);

            return Result<RoomDto>.Success(new RoomDto(
                room.Id,
                room.Name,
                room.Capacity,
                room.PricePerHour,
                room.Services.Select(s => new RoomServiceDto(
                    s.Id,
                    s.Name,
                    s.Price
                )).ToList()
            ));
        }
    }

    public record UpdateRoomRequest(
        string Name,
        int Capacity,
        decimal PricePerHour,
        List<Guid> ServiceIds);

    public class UpdateConferenceRoomEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/rooms/{roomId:guid}", async (
                [FromRoute] Guid roomId,
                UpdateRoomRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(new UpdateConferenceRoomCommand(
                    roomId,
                    request.Name,
                    request.Capacity,
                    request.PricePerHour,
                    request.ServiceIds ?? []
                ));

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return result.Error?.StatusCode switch
                {
                    StatusCodes.Status404NotFound => Results.NotFound(result.Error.Message),
                    StatusCodes.Status400BadRequest => Results.BadRequest(result.Error.Message),
                    _ => Results.Problem(result.Error?.Message)
                };
            })
            .WithName("UpdateRoomInformation")
            .WithTags("Rooms")
            .WithSummary("Updates existing conference room details")
            .WithDescription("Updates the name, capacity, price, and assigned services of a specific conference room by its unique ID.");
        }
    }

    public class UpdateConferenceRoomValidator : AbstractValidator<UpdateConferenceRoomCommand>
    {
        public UpdateConferenceRoomValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(r => r.PricePerHour)
                .NotNull()
                .GreaterThan(0);

            RuleFor(r => r.Capacity)
                .GreaterThan(0);
        }
    }
}

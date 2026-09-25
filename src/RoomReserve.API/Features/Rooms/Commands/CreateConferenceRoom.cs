using MediatR;
using RoomReserve.Application.BusinessLogic.Rooms.Dtos;
using RoomReserve.API.Models;
using System;
using System.Collections.Generic;
using System.Text;
using RoomReserve.API.Infrastructure.Persistence;
using RoomReserve.API.Models.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace RoomReserve.Application.BusinessLogic.Rooms.Commands
{
    public record CreateConferenceRoomCommand(
        string Name,
        int Capacity,
        decimal PricePerHour,
        List<Guid> ServiceIds) : IRequest<Result<Guid>>;

    public class CreateConferenceRoomCommandHandler(
        ApplicationDbContext context,
        ILogger<CreateConferenceRoomCommandHandler> logger) : IRequestHandler<CreateConferenceRoomCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateConferenceRoomCommand request, CancellationToken cancellationToken)
        {
            var services = await context.Services
                .Where(s => request.ServiceIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            var missingServices = request.ServiceIds.Except(services.Select(s => s.Id)).ToList();

            if (missingServices.Any())
            {
                logger.LogWarning("Some services are missing: {MissingServices}", missingServices);
                return Result<Guid>.Failure(ResultError.BadRequest($"Some services are missing: {string.Join(", ", missingServices)}"));
            }

            var room = ConferenceRoom.Create(request.Name, request.Capacity, request.PricePerHour);

            context.ConferenceRooms.Add(room);
            await context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(room.Id);
        }
    }

    public record CreateRoomRequest(
        string Name,
        int Capacity,
        decimal PricePerHour,
        List<Guid> ServiceIds);

    public class CreateConferenceRoomEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/rooms", async (
                CreateRoomRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(new CreateConferenceRoomCommand(
                    request.Name,
                    request.Capacity,
                    request.PricePerHour,
                    request.ServiceIds
                ));

                if (result.IsSuccess)
                {
                    return Results.Created($"/api/rooms/{result.Value}", new
                    {
                        Id = result.Value
                    });
                }

                return Results.BadRequest(result.Error?.Message);
            })
            .WithName("CreateRoom")
            .WithTags("Rooms")
            .WithSummary("Creates a new conference room")
            .WithDescription("Creates a new conference room with the specified name, capacity, price, and assigned services.");
        }
    }

    public class CreateConferenceRoomValidator : AbstractValidator<CreateConferenceRoomCommand>
    {
        public CreateConferenceRoomValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(r => r.PricePerHour)
                .GreaterThan(0);

            RuleFor(r => r.Capacity)
                .GreaterThan(0);
        }
    }
}

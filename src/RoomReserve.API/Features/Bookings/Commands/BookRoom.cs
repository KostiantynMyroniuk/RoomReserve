using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomReserve.API.Features.Bookings.Dtos;
using RoomReserve.API.Infrastructure.Persistence;
using RoomReserve.API.Models;
using RoomReserve.API.Models.Common;
using RoomReserve.Application.BusinessLogic.Bookings.Dtos;
using RoomReserve.Application.BusinessLogic.Rooms.Commands;
using RoomReserve.Application.BusinessLogic.Rooms.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Application.BusinessLogic.Bookings.Commands
{
    public record BookRoomCommand(
        Guid RoomId,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        List<Guid> ServiceIds) : IRequest<Result<BookingDto>>;

    public class BookRoomCommandHandler(
        ApplicationDbContext context,
        ILogger<BookRoomCommandHandler> logger) : IRequestHandler<BookRoomCommand, Result<BookingDto>>
    {
        public async Task<Result<BookingDto>> Handle(BookRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await context.ConferenceRooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);

            if (room == null)
            {
                return Result<BookingDto>.Failure(ResultError.NotFound($"Room with id {request.RoomId} not found."));
            }

            var services = await context.Services
                .AsNoTracking()
                .Where(s => request.ServiceIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            var missingServices = request.ServiceIds.Except(services.Select(s => s.Id)).ToList();

            if (missingServices.Any())
            {
                logger.LogWarning("Some services are missing: {MissingServices}", missingServices);
                return Result<BookingDto>.Failure(ResultError.BadRequest($"Some services are missing: {string.Join(", ", missingServices)}"));
            }

            var hasOverlap = await context.Bookings
                .Where(b => b.RoomId == request.RoomId && b.Date == request.Date)
                .AnyAsync(b => request.StartTime < b.EndTime && request.EndTime > b.StartTime, cancellationToken);

            if (hasOverlap)
            {
                return Result<BookingDto>.Failure(ResultError.Conflict("Room is already booked for this time slot."));
            }

            var booking = Booking.Create(
                request.RoomId,
                request.Date,
                request.StartTime,
                request.EndTime,
                room.PricePerHour,
                services);

            context.Bookings.Add(booking);
            await context.SaveChangesAsync(cancellationToken);

            return Result<BookingDto>.Success(new BookingDto(
                booking.Id,
                booking.Date,
                booking.StartTime,
                booking.EndTime,
                booking.TotalPrice));
        }
    }

    public record BookRoomRequest(
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        List<Guid> ServiceIds);

    public class BookRoomEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/rooms/{roomId}/bookings", async (
                Guid roomId,
                BookRoomRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(new BookRoomCommand(
                    roomId,
                    request.Date,
                    request.StartTime,
                    request.EndTime,
                    request.ServiceIds ?? []
                ));

                if (result.IsSuccess)
                    return Results.Ok(result.Value);

                return result.Error?.StatusCode switch
                {
                    StatusCodes.Status404NotFound => Results.NotFound(result.Error.Message),
                    StatusCodes.Status409Conflict => Results.Conflict(result.Error.Message),
                    StatusCodes.Status400BadRequest => Results.BadRequest(result.Error.Message),
                    _ => Results.Problem(result.Error?.Message)
                };
            })
            .WithName("BookRoom")
            .WithTags("Bookings");
        }
    }

    public class BookRoomValidator : AbstractValidator<BookRoomCommand>
    {
        public BookRoomValidator()
        {
            RuleFor(r => r.StartTime)
                .NotEmpty()
                .LessThan(r => r.EndTime);

            RuleFor(r => r.EndTime)
                .NotEmpty();
        }
    }
}

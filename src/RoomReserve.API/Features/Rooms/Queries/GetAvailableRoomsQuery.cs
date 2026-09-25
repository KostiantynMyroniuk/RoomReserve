using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReserve.API.Infrastructure.Persistence;
using RoomReserve.API.Models.Common;
using RoomReserve.Application.BusinessLogic.Rooms.Commands;
using RoomReserve.Application.BusinessLogic.Rooms.Dtos;
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
        int? Capacity) : IRequest<PaginatedList<RoomDto>>;

    public class GetAvailableRoomsQueryHandler(
        ApplicationDbContext context) : IRequestHandler<GetAvailableRoomsQuery, PaginatedList<RoomDto>>
    {
        public async Task<PaginatedList<RoomDto>> Handle(GetAvailableRoomsQuery request, CancellationToken cancellationToken)
        {
            var query = context.ConferenceRooms
                .AsNoTracking();

            //date and time filter 
            if (request.Date.HasValue && request.StartTime.HasValue && request.EndTime.HasValue)
            {
                query = query.Where(r => !r.Bookings.Any(b =>
                    b.Date == request.Date.Value &&
                    b.StartTime < request.EndTime.Value &&
                    b.EndTime > request.StartTime.Value));
            }

            //capacity filter
            if (request.Capacity.HasValue)
            {
                query = query.Where(r => r.Capacity >= request.Capacity.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            //pagination
            var roomsPaginated = await query
                .OrderBy(r => r.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new RoomDto(
                    r.Id,
                    r.Name,
                    r.Capacity,
                    r.PricePerHour,
                    r.Services.Select(s => new RoomServiceDto(
                        s.Id, 
                        s.Name, 
                        s.Price)).ToList()
                ))
                .ToListAsync(cancellationToken);

            return new PaginatedList<RoomDto>(roomsPaginated, request.PageNumber, request.PageSize, totalCount);
        }
    }

    public record GetAvailableRoomsRequest(
        int PageNumber = 1,
        int PageSize = 10,
        DateOnly? Date = null,
        TimeOnly? StartTime = null,
        TimeOnly? EndTime = null,
        int? Capacity = null);

    public class GetAvailableRoomsEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/rooms/", async (
                [AsParameters] GetAvailableRoomsRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(new GetAvailableRoomsQuery(
                    request.PageNumber,
                    request.PageSize,
                    request.Date,
                    request.StartTime,
                    request.EndTime,
                    request.Capacity
                ));

                return Results.Ok(result);
            })
            .WithName("GetAvailableRooms")
            .WithTags("Rooms");
        }
    }

    public class GetAvailableRoomsValidator : AbstractValidator<GetAvailableRoomsQuery>
    {
        public GetAvailableRoomsValidator()
        {
            RuleFor(r => r.PageNumber)
                .GreaterThan(0);

            RuleFor(r => r.PageSize)
                .GreaterThan(0);

            RuleFor(r => r.StartTime)
                .LessThan(r => r.EndTime);

            RuleFor(r => r.Capacity)
                .GreaterThan(0);
        }
    }
}

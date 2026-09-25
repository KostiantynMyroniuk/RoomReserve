using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomReserve.API.Features.Analytics.Dtos;
using RoomReserve.API.Infrastructure.Persistence;
using RoomReserve.API.Models.Common;

namespace RoomReserve.API.Features.Analytics.Queries
{
    public record GetRevenueSummaryQuery(
        DateOnly StartDate,
        DateOnly EndDate) : IRequest<RevenueSummaryDto>;

    public class GetRevenueSummaryQueryHandler(
        ApplicationDbContext context) : IRequestHandler<GetRevenueSummaryQuery, RevenueSummaryDto>
    {
        public async Task<RevenueSummaryDto> Handle(GetRevenueSummaryQuery request, CancellationToken cancellationToken)
        {
            var roomRevenues = await context.ConferenceRooms
                .AsNoTracking()
                .Select(r => new
                {
                    r.Id,
                    r.Name,

                    BookingCount = r.Bookings.Count(b =>
                        b.Date >= request.StartDate &&
                        b.Date <= request.EndDate),

                    TotalRoomRevenue = r.Bookings
                        .Where(b =>
                            b.Date >= request.StartDate &&
                            b.Date <= request.EndDate)
                        .Sum(b => b.TotalPrice)
                })
                .OrderByDescending(r => r.TotalRoomRevenue)
                .Select(r => new RoomRevenueDto(
                    r.Id,
                    r.Name,
                    r.BookingCount,
                    r.TotalRoomRevenue))
                .ToListAsync(cancellationToken);

            var revenueSummary = new RevenueSummaryDto(
                roomRevenues,
                roomRevenues.Sum(r => r.TotalRoomRevenue));

            return revenueSummary;
        }
    }

    public record GetRevenueSummaryRequest(
        DateOnly StartDate,
        DateOnly EndDate);

    public class GetRevenueSummaryEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/analytics/revenue", async (
                [AsParameters] GetRevenueSummaryRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(new GetRevenueSummaryQuery(
                    request.StartDate,
                    request.EndDate));

                return Results.Ok(result);
            })
            .WithName("GetRevenueSummary")
            .WithTags("Analytics")
            .WithSummary("Get the revenue summary within a specified date range.")
            .WithDescription("Gets the revenue summary for the specified date range.");
        }
    }

    public class GetRevenueSummaryValidator : AbstractValidator<GetRevenueSummaryQuery>
    {
        public GetRevenueSummaryValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .LessThan(x => x.EndDate);

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .GreaterThan(x => x.StartDate);
        }
    }
}

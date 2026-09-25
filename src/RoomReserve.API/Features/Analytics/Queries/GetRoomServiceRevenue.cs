using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomReserve.API.Features.Analytics.Dtos;
using RoomReserve.API.Infrastructure.Persistence;
using RoomReserve.API.Models.Common;

namespace RoomReserve.API.Features.Analytics.Queries
{
    public record GetRoomServiceRevenueQuery(
        DateOnly StartDate,
        DateOnly EndDate) : IRequest<IEnumerable<RoomServiceRevenueDto>>;


    public class GetRoomServiceRevenueQueryHandler(
        ApplicationDbContext context) : IRequestHandler<GetRoomServiceRevenueQuery, IEnumerable<RoomServiceRevenueDto>>
    {
        public async Task<IEnumerable<RoomServiceRevenueDto>> Handle(GetRoomServiceRevenueQuery request, CancellationToken cancellationToken)
        {
            var serviceRevenue = await context.Bookings
                .AsNoTracking()
                .Where(b =>
                    b.Date >= request.StartDate &&
                    b.Date <= request.EndDate)
                .SelectMany(b => b.BookingServices)
                .GroupBy(bs => new
                {
                    bs.ServiceId,
                    bs.ServiceName
                })
                .Select(g => new
                {
                    g.Key.ServiceId,
                    g.Key.ServiceName,
                    UsageCount = g.Count(),
                    TotalRevenue = g.Sum(bs => bs.Price)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Select(x => new RoomServiceRevenueDto(
                    x.ServiceId,
                    x.ServiceName,
                    x.UsageCount,
                    x.TotalRevenue))
                .ToListAsync(cancellationToken);

            return serviceRevenue;  
        }
    }

    public record GetRoomServiceRevenueRequest(
        DateOnly StartDate,
        DateOnly EndDate);

    public class GetRoomServiceRevenueEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/analytics/service-revenue", async (
                [AsParameters] GetRoomServiceRevenueRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(new GetRoomServiceRevenueQuery(
                    request.StartDate,
                    request.EndDate));

                return Results.Ok(result);
            })
            .WithName("GetRoomServiceRevenue")
            .WithTags("Analytics")
            .WithSummary("Get the room services revenue summary within a specified date range.")
            .WithDescription("Gets the room services revenue summary for the specified date range.");
        }
    }
}

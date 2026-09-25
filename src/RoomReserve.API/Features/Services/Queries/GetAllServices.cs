using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomReserve.API.Features.Services.Dtos;
using RoomReserve.API.Infrastructure.Persistence;
using RoomReserve.API.Models.Common;

namespace RoomReserve.API.Features.Services.Queries
{
    public record GetAllServicesQuery : IRequest<IEnumerable<ServiceDto>>;

    public class GetAllServicesQueryHandler(
        ApplicationDbContext context) : IRequestHandler<GetAllServicesQuery, IEnumerable<ServiceDto>>
    {
        public async Task<IEnumerable<ServiceDto>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
        {
            var services = await context.Services
                .AsNoTracking()
                .Select(s => new ServiceDto(
                    s.Id,
                    s.Name,
                    s.Price))
                .ToListAsync(cancellationToken);

            return services;
        }
    }

    public class GetAllServicesEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/services", async (
                ISender sender) =>
            {
                var result = await sender.Send(new GetAllServicesQuery());

                return Results.Ok(result);
            })
            .WithName("GetAllServices")
            .WithTags("Services")
            .WithSummary("Retrieves a list of all services")
            .WithDescription("Retrieves a list of all available services in the system.");
        }
    }
}

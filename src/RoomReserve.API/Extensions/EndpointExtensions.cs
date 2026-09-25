using Microsoft.Extensions.DependencyInjection.Extensions;
using RoomReserve.API.Models.Common;
using System.Reflection;

namespace RoomReserve.API.Extensions
{
    public static class EndpointExtensions
    {
        public static void AddEndpoints(this IServiceCollection services, Assembly assembly)
        {
            var endpointTypes = assembly.DefinedTypes
                .Where(t => t is { IsAbstract: false, IsInterface: false } && t.IsAssignableTo(typeof(IEndpoint)))
                .Select(t => new ServiceDescriptor(typeof(IEndpoint), t, ServiceLifetime.Scoped));

            services.TryAddEnumerable(endpointTypes);
        }

        public static void MapEndpoints(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var endpoints = scope.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>();

            foreach (var endpoint in endpoints)
            {
                endpoint.MapEndpoint(app);
            }
        }
    }
}

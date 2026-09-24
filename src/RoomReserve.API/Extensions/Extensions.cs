using Microsoft.EntityFrameworkCore;
using RoomReserve.API.Infrastructure.Persistence;

namespace RoomReserve.API.Extensions
{
    public static class Extensions
    {
        public static void AddPersistence(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("RoomReserveDb")
                    ?? throw new InvalidOperationException("Property 'RoomReserveDb' is not configured.")));
        }

        public static void AddServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Extensions).Assembly);
            });
        }
    }
}

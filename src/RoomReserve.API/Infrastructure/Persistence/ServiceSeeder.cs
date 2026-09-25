using RoomReserve.API.Models;

namespace RoomReserve.API.Infrastructure.Persistence
{
    public static class ServiceSeeder
    {
        public static async Task SeedServices(this WebApplication app)
        {
            using var scope = app.Services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Services.Any())
            {
                var services = new List<Service>()
                {
                    Service.Create("Projector", 500.0m),
                    Service.Create("Wi-Fi", 300.0m),
                    Service.Create("Sound", 700.0m)
                };

                await context.Services.AddRangeAsync(services);
                await context.SaveChangesAsync();
            }
        }
    }
}

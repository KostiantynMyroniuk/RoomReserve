using RoomReserve.API.Models;

namespace RoomReserve.API.Infrastructure.Persistence
{
    public static class RoomSeeder
    {
        public static async Task SeedRooms(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.ConferenceRooms.Any())
            {
                var rooms = new List<ConferenceRoom>
                {
                    ConferenceRoom.Create(name: "Room A", capacity: 50, pricePerHour: 2000.0m),
                    ConferenceRoom.Create(name: "Room B", capacity: 100, pricePerHour: 3500.0m),
                    ConferenceRoom.Create(name: "Room C", capacity: 30, pricePerHour: 1500.0m)
                };

                context.ConferenceRooms.AddRange(rooms);
                await context.SaveChangesAsync();
            }
        }
    }
}

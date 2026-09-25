using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using RoomReserve.API.Infrastructure.Persistence;
using RoomReserve.API.Middlewares;
using System.Text.Json.Nodes;

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

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
        }

        public static void AddSwaggerWithOptions(this IHostApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.MapType<DateOnly>(() => new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Format = "date",
                    Example = JsonValue.Create(DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd"))
                });

                options.MapType<TimeOnly>(() => new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Format = "time",
                    Example = JsonValue.Create(TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm:ss"))
                });
            });
        }
    }
}

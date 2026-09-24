using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomReserve.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Infrastructure.Extensions
{
    public static class Extensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("RoomReserveDb"));
            });
        }
    }
}

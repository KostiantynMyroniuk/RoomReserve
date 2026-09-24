using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReserve.API.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Infrastructure.Persistence.Configurations
{
    public class ServiceConfigurations : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.Property(s => s.Name)
                .HasMaxLength(200);

            builder.Property(s => s.Price)
                .HasPrecision(18, 2);
        }
    }
}

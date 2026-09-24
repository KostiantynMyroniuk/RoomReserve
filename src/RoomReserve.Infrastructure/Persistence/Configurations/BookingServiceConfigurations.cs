using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReserve.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Infrastructure.Persistence.Configurations
{
    public class BookingServiceConfigurations : IEntityTypeConfiguration<BookingService>
    {
        public void Configure(EntityTypeBuilder<BookingService> builder)
        {
            builder.Property(bs => bs.ServiceName)
                .HasMaxLength(200);

            builder.Property(bs => bs.Price)
                .HasPrecision(18, 2);
        }
    }
}

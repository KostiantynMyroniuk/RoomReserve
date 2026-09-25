using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReserve.API.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Infrastructure.Persistence.Configurations
{
    public class BookingConfigurations : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Property(b => b.TotalPrice)
                .HasPrecision(18, 2);

            builder.Property(b => b.RoomPricePerHour)
                .HasPrecision(18, 2);

            builder
                .HasMany(b => b.BookingServices)
                .WithOne(bs => bs.Booking)
                .HasForeignKey(bs => bs.BookingId);
        }
    }
}

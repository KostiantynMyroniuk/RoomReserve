using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReserve.API.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Infrastructure.Persistence.Configurations
{
    public class ConferenceRoomConfigurations : IEntityTypeConfiguration<ConferenceRoom>
    {
        public void Configure(EntityTypeBuilder<ConferenceRoom> builder)
        {
            builder.Property(r => r.Name)
                .HasMaxLength(200);

            builder.Property(r => r.PricePerHour)
                .HasPrecision(18, 2);

            builder
                .HasMany(r => r.Services)
                .WithMany();

            builder
                .HasMany(r => r.Bookings)
                .WithOne()
                .HasForeignKey(b => b.RoomId);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReserve.API.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace RoomReserve.Infrastructure.Persistence.Configurations
{
    public class BookingServiceConfigurations : IEntityTypeConfiguration<BookingService>
    {
        public void Configure(EntityTypeBuilder<BookingService> builder)
        {
            builder
                .HasKey(bs => new { bs.BookingId, bs.ServiceId });

            builder.Property(bs => bs.ServiceName)
                .HasMaxLength(200);

            builder.Property(bs => bs.Price)
                .HasPrecision(18, 2);

            builder
                .HasOne(bs => bs.Booking)     
                .WithMany(b => b.BookingServices) 
                .HasForeignKey(bs => bs.BookingId);

            builder
                .HasOne(bs => bs.Service)         
                .WithMany() 
                .HasForeignKey(bs => bs.ServiceId);
        }
    }
}

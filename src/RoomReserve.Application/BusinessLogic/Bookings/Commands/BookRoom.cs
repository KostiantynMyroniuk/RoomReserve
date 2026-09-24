using MediatR;
using RoomReserve.Application.BusinessLogic.Bookings.Dtos;
using RoomReserve.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Application.BusinessLogic.Bookings.Commands
{
    public record BookRoomCommand(
        Guid RoomId,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime) : IRequest<BookingDto>;

    public class BookRoomCommandHandler(
        IBookingRepository bookingRepository) : IRequestHandler<BookRoomCommand, BookingDto>
    {
        public Task<BookingDto> Handle(BookRoomCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

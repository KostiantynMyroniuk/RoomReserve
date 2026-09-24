namespace RoomReserve.API.Features.Bookings.Dtos
{
    public record BookingServiceDto(Guid ServiceId, string Name, decimal Price);
}

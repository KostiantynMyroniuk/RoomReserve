namespace RoomReserve.API.Features.Analytics.Dtos
{
    public record RoomRevenueDto(
        Guid RoomId,
        string ConferenceRoomName,
        int BookingCount,
        decimal TotalRoomRevenue);

    public record RevenueSummaryDto(
        List<RoomRevenueDto> RoomRevenues,
        decimal TotalRevenue);
}

namespace RoomReserve.API.Features.Analytics.Dtos
{
    public record RoomServiceRevenueDto(
        Guid ServiceId,
        string ServiceName,
        int UsageCount,
        decimal TotalRevenue);
}

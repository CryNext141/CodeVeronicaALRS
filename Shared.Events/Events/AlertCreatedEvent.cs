namespace Shared.Events
{
    public record AlertCreatedEvent(
        int AlertId,
        string RegionName,
        string Summary
    );
}

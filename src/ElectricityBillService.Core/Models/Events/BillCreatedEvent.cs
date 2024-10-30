namespace ElectricityBillService.Core.EventHandling.Events
{
    public record BillCreatedEvent(string BillId, decimal Amount);

}

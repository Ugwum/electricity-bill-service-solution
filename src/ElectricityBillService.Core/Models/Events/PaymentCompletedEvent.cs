namespace ElectricityBillService.Core.EventHandling.Events
{
    public record PaymentCompletedEvent(string BillId, string walletID ,decimal Amount);

}

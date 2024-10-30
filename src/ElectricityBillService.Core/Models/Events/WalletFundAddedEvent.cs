namespace ElectricityBillService.Core.EventHandling.Events
{
    public record WalletFundAddedEvent(Guid WalletId, decimal Amount);

}

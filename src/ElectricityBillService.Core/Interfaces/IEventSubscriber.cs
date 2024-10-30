namespace ElectricityBillService.Infrastructure
{
    public interface IEventSubscriber
    {
        Task SubscribeAsync( Func<string, Task> onMessageReceieved, string queueurl = null); 
    }

}

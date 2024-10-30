namespace ElectricityBillService.Infrastructure
{
    public interface ISmsService
    {
        Task SendSmsAsync(string message, string phoneNumber);
    }
}
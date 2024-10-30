using Amazon.SQS;
using Amazon.SQS.Model;
using ElectricityBillService.Core.EventHandling.Events;
using ElectricityBillService.Core.Interfaces;
using ElectricityBillService.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ElectricityBillService.Infrastructure
{
    public class FundWalletEventSubscriber : IEventSubscriber
    {
        private readonly IAmazonSQS _sqsClient; 
        private readonly ILogger<FundWalletEventSubscriber> _logger;
        private readonly IWalletRepository _walletRepository;
        private readonly ISmsService _smsService;
        private readonly IConfiguration _config;
        public FundWalletEventSubscriber(IAmazonSQS sqsClient, ILogger<FundWalletEventSubscriber> logger,
            ISmsService smsService, IWalletRepository walletRepository, IConfiguration config)
        {
            _sqsClient = sqsClient;
            _logger = logger;
            _smsService = smsService;
            _walletRepository = walletRepository;
            _config = config;
        }
        public async Task SubscribeAsync( Func<string, Task> onMessageReceieved = null, string queueUrl = null)
        {
            try
            {
                var receiveRequest = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    WaitTimeSeconds = 5,
                    MaxNumberOfMessages = 10
                };

                var messages = await _sqsClient.ReceiveMessageAsync(receiveRequest);

                foreach (var message in messages.Messages)
                {
                    await onMessageReceieved(message.Body);
                    await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                }
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"An error occurred subscribing to to FundWallet Event {ex.Message}");
            }
        }

        public async Task HandleMessageAsync(string messageBody)
        {
            var fundsEvent = JsonConvert.DeserializeObject<WalletFundAddedEvent>(messageBody);
            await HandleFundsAddedEvent(fundsEvent);
        }

        private async Task HandleFundsAddedEvent(WalletFundAddedEvent fundsEvent)
        {
            try
            {
                _logger.LogInformation($"Processing FundsAddedEvent for WalletId: {fundsEvent.WalletId}");
                var wallet = await _walletRepository.GetWalletByIdAsync(fundsEvent.WalletId.ToString());

                if(wallet != null)
                { 
                    await _smsService.SendSmsAsync($"Wallet funded with {fundsEvent.Amount}. Available balance is {wallet.Balance}", wallet.PhoneNumber);

                    if (wallet.Balance < Convert.ToDecimal(_config["SmsSettings:BalanceThreshold"]))
                    {
                        await _smsService.SendSmsAsync("Low balance alert", wallet.PhoneNumber);
                    }
                }
                 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing FundsAddedEvent: {ex.Message}");
            }
        }
    }
}

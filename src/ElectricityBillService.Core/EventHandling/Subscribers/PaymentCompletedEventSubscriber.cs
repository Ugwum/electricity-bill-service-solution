using Amazon.SQS.Model;
using Amazon.SQS;
using ElectricityBillService.Core.EventHandling.Events;
using ElectricityBillService.DataAccess.Repositories;
using ElectricityBillService.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricityBillService.Core.Models;
using ElectricityBillService.Infrastructure.Provider;

namespace ElectricityBillService.Core.EventHandling.Subscribers
{ 
    public class PaymentCompletedEventSubscriber : IEventSubscriber
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger<FundWalletEventSubscriber> _logger;
        private readonly IWalletRepository _walletRepository;
        private readonly IBillRepository _billRepository;
        private readonly ISmsService _smsService;
        private readonly IConfiguration _config;
        private readonly IElectricityProvider _electricityProvider;
        public PaymentCompletedEventSubscriber(IAmazonSQS sqsClient, ILogger<FundWalletEventSubscriber> logger,
            ISmsService smsService, IWalletRepository walletRepository, 
            IBillRepository billRepository, IConfiguration config, IElectricityProvider electricityProvider)
        {
            _sqsClient = sqsClient;
            _logger = logger;
            _smsService = smsService;
            _walletRepository = walletRepository;
            _billRepository = billRepository;
            _electricityProvider = electricityProvider;
            _config = config;
        }
        public async Task SubscribeAsync(Func<string, Task> onMessageReceieved = null, string queueUrl = null)
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
            catch (Exception ex)
            {
                _logger.LogInformation($"An error occurred subscribing to to FundWallet Event {ex.Message}");
            }
        }

        public async Task HandleMessageAsync(string messageBody)
        {
            var paymentCompletedEvent = JsonConvert.DeserializeObject<PaymentCompletedEvent>(messageBody);
            await ProcessBillWithProvider(paymentCompletedEvent);
            await HandleSuccessfulPaymentEvent(paymentCompletedEvent);
        }

        private async Task HandleSuccessfulPaymentEvent(PaymentCompletedEvent paymentCompletedEvent)
        {
            try
            {
                _logger.LogInformation($"Processing Successful bill payment for bill {paymentCompletedEvent.BillId} with Wallet WalletId: {paymentCompletedEvent.walletID}");

                var wallet = await _walletRepository.GetWalletByIdAsync(paymentCompletedEvent.walletID.ToString());
                
                if (wallet != null)
                {  
                    // Send SMS notification
                    try { await _smsService.SendSmsAsync($"Your payment of {paymentCompletedEvent.Amount}  for bill {paymentCompletedEvent.BillId} was successful.", wallet.PhoneNumber); } catch { }

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

        private async Task ProcessBillWithProvider(PaymentCompletedEvent paymentCompletedEvent)
        {
            try
            {
                var bill = await _billRepository.GetByValidationRefAsync(paymentCompletedEvent.BillId);
                await _electricityProvider.ProcessBillPaymentAsync(bill);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred processing bill with provider: {ex.Message}");
            }
        }
    }

}

using Amazon.SQS;
using Amazon.SQS.Model;
using ElectricityBillService.Core.EventHandling.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ElectricityBillService.Infrastructure
{
    public class BillCreatedEventSubscriber : IEventSubscriber
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger<BillCreatedEventSubscriber> _logger;

        public BillCreatedEventSubscriber(IAmazonSQS sqsClient, ILogger<BillCreatedEventSubscriber> logger)
        {
            _sqsClient = sqsClient;
            _logger = logger;
        }

        public async Task SubscribeAsync( Func<string, Task> onMessageReceived , string queueUrl)
        {
            try
            {
                while (true)
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
                        await onMessageReceived(message.Body);
                        await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"An error occurred subscribing to Bill created Event {ex.Message}");
            }
            
        }

        public async Task HandleMessageAsync(string messageBody)
        {
            var billEvent = JsonConvert.DeserializeObject<BillCreatedEvent>(messageBody);
            await HandleBillCreatedEvent(billEvent);
        }

        private async Task HandleBillCreatedEvent(BillCreatedEvent billEvent)
        {
            _logger.LogInformation($"Processing BillCreatedEvent for BillId: {billEvent.BillId}");
            // Handle bill creation logic
        }
    }


}

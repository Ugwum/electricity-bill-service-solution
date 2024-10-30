using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;

namespace ElectricityBillService.Infrastructure.SmsNotification
{
    public class SetupLocalStack
    {
        private readonly IAmazonSimpleNotificationService _snsClient;
        private readonly IAmazonSQS _sqsClient;
        private readonly IConfiguration _config;

        public SetupLocalStack(IAmazonSimpleNotificationService snsClient, IAmazonSQS sqsClient, IConfiguration config)
        {
            _snsClient = snsClient;
            _sqsClient = sqsClient;
            _config = config;
        }

        public async Task InitializeAsync()
        {
            // Create SNS topic for FundsAddedEvent
            var billservicetopic = await _snsClient.CreateTopicAsync(_config["Messaging:SNSTopic"]);
            var billservicetopicArn = billservicetopic.TopicArn;

            var queueUrls = new List<string>();

            // Create SQS queue for wallet service to listen to funds added
            var fundsAddedQueue = await _sqsClient.CreateQueueAsync(_config["Messaging:FundsAddedQueue"]);
            queueUrls.Add(fundsAddedQueue.QueueUrl);

            // Create SQS queue for wallet service to listen to funds added
            var successfulPaymentQueue = await _sqsClient.CreateQueueAsync(_config["Messaging:SuccessfulPaymentQueue"]);
            queueUrls.Add(successfulPaymentQueue.QueueUrl);

            var billscreatedQueue = await _sqsClient.CreateQueueAsync(_config["Messaging:BillCreatedQueue"]);
            queueUrls.Add(billscreatedQueue.QueueUrl);

            foreach (var queueUrl in queueUrls) 
            {
                // Subscribe SQS queue to SNS topic
                await _snsClient.SubscribeQueueAsync(billservicetopicArn, _sqsClient, queueUrl);
            }

           
        }
    }
}

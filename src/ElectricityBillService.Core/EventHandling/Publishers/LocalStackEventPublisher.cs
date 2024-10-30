using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ElectricityBillService.Infrastructure.EventPublishers
{
    public class LocalStackEventPublisher : IEventPublisher
    {
        private readonly IConfiguration _config;

        public LocalStackEventPublisher(IConfiguration config)
        {
            _config = config;
        }

        public async Task PublishAsync<T>(T eventMessage) where T : class
        {
            // Use LocalStack SNS client setup
            var client = new AmazonSimpleNotificationServiceClient(new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = _config["LocalStack:ServiceURL"]
            });

            CreateTopicResponse createTopicResponse = await client.CreateTopicAsync(_config["Messaging:SNSTopic"]);
            string topicArn = createTopicResponse.TopicArn;

            await client.PublishAsync(new PublishRequest
            {
                TopicArn = topicArn,
                Message = JsonConvert.SerializeObject(eventMessage)
            });
        }
    }
}

using Amazon.SQS;
using ElectricityBillService.Core.EventHandling.Subscribers;
using ElectricityBillService.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityBillService.Core
{
    public class BackgroundSubscriberInitializer
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundSubscriberInitializer(IHostApplicationLifetime lifetime, IServiceProvider serviceProvider)
        {
            _lifetime = lifetime;
            _serviceProvider = serviceProvider;
        }

        public async Task StartBackgroundSubscribers()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServiceProvider = scope.ServiceProvider;

                var fundsAddedSubscriber = scopedServiceProvider.GetRequiredService<FundWalletEventSubscriber>();
                var paymentSuccessfulSubscriber = scopedServiceProvider.GetRequiredService<PaymentCompletedEventSubscriber>();
                var billCreatedSubscriber = scopedServiceProvider.GetRequiredService<BillCreatedEventSubscriber>();

                var _sqsClient = scopedServiceProvider.GetRequiredService<IAmazonSQS>();
                var _config = scopedServiceProvider.GetRequiredService<IConfiguration>();

                var fundsAddedQueueUrl = await _sqsClient.GetQueueUrlAsync(_config["Messaging:FundsAddedQueue"]);
                var billCreatedQueueUrl = await _sqsClient.GetQueueUrlAsync(_config["Messaging:BillCreatedQueue"]);
                var successfulpaymentQueueUrl = await _sqsClient.GetQueueUrlAsync(_config["Messaging:SuccessfulPaymentQueue"]);

                _lifetime.ApplicationStarted.Register(() =>
                {
                    // Start funds added subscriber in the background
                    Task.Run(() => fundsAddedSubscriber.SubscribeAsync(fundsAddedSubscriber.HandleMessageAsync, fundsAddedQueueUrl.QueueUrl));

                    // Start bill created subscriber in the background
                    Task.Run(() => billCreatedSubscriber.SubscribeAsync(billCreatedSubscriber.HandleMessageAsync, billCreatedQueueUrl.QueueUrl));

                    // Start payment successful subscriber in the background
                    Task.Run(() => paymentSuccessfulSubscriber.SubscribeAsync(paymentSuccessfulSubscriber.HandleMessageAsync, successfulpaymentQueueUrl.QueueUrl));
                });
            }
        }
    }
}

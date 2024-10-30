using Amazon.SimpleNotificationService;
using Amazon.SQS;
using ElectricityBillService.Core.Interfaces;
using ElectricityBillService.Core.Services;
using ElectricityBillService.Infrastructure.EventPublishers;
using ElectricityBillService.Infrastructure.SmsNotification;
using ElectricityBillService.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricityBillService.DataAccess.Repositories;
using ElectricityBillService.Infrastructure.Provider;
using ElectricityBillService.Core.EventHandling.Subscribers;
using Amazon.Runtime;

namespace ElectricityBillService.Core
{
    public class ServiceInitializer
    {
        private readonly IConfiguration  _config;

        public ServiceInitializer(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void RegisterServices(IServiceCollection services)
        {
            // Register AWS SNS and SQS clients             

            services.AddSingleton<IAmazonSimpleNotificationService>(new AmazonSimpleNotificationServiceClient(
                new BasicAWSCredentials(_config["AWSCredential:AccessKey"], _config["AWSCredential:SecretKey"]),
                new AmazonSimpleNotificationServiceConfig
                {
                    ServiceURL = _config["LocalStack:ServiceURL"]
                }
            ));

            services.AddSingleton<IAmazonSQS>(new AmazonSQSClient(
                new BasicAWSCredentials(_config["AWS:AccessKey"], _config["AWS:SecretKey"]),
                new AmazonSQSConfig
                {
                    ServiceURL = _config["LocalStack:ServiceURL"]
                }
            ));


            //services.AddSingleton<IAmazonSimpleNotificationService>(new AmazonSimpleNotificationServiceClient(new AmazonSimpleNotificationServiceConfig
            //{
            //    ServiceURL = _config["LocalStack:ServiceURL"]
            //}));

            //services.AddSingleton<IAmazonSQS>(new AmazonSQSClient(new AmazonSQSConfig
            //{
            //    ServiceURL = _config["LocalStack:ServiceURL"]
            //}));

            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IBillService, BillService>();
            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();

            // Register the mock SMS service for development
         //   services.AddSingleton<ISmsService, MockSmsService>();

            services.AddScoped<IElectricityProvider, ProviderSelectorService>();

            // register the Twilio service 
             services.AddSingleton<ISmsService>(provider => 
                 new TwilioSmsService(_config["SmsSettings:TwilioSID"], _config["SmsSettings:TwilioToken"], _config["SmsSettings:TwilioPhoneNumber"]));


            // Configure event publishers and subscribers
            services.AddSingleton<IEventPublisher, LocalStackEventPublisher>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<FundWalletEventSubscriber>();

            services.AddScoped<BillCreatedEventSubscriber>();
            services.AddScoped<PaymentCompletedEventSubscriber>();
            services.AddSingleton<SetupLocalStack>();

            services.AddScoped<MockProviderA>();
            services.AddScoped<MockProviderB>();
 
        }

        public async Task InitializeLocalStackResources(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var setupLocalStack = scope.ServiceProvider.GetRequiredService<SetupLocalStack>();
                await setupLocalStack.InitializeAsync();
            }
        }
    }
}

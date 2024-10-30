
using ElectricityBillService.Core;
using ElectricityBillService.Core.Interfaces;
using ElectricityBillService.Core.Services;
using ElectricityBillService.DataAccess;
using ElectricityBillService.DataAccess.Repositories;
using ElectricityBillService.Infrastructure;
using ElectricityBillService.Infrastructure.SmsNotification;
using Microsoft.EntityFrameworkCore;

namespace ElectricityBillService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
 

            // Load appsettings.json configuration
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


            // Initialize and register services using ServiceInitializer

            builder.Services.AddDbContext<ElectricityBillDBContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 26)))); 


            var serviceInitializer = new ServiceInitializer(builder.Configuration);
            serviceInitializer.RegisterServices(builder.Services);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            

            var app = builder.Build();

            // Initialize LocalStack SNS and SQS resources
            await serviceInitializer.InitializeLocalStackResources(app.Services);

            // Start background tasks for event subscribers using BackgroundSubscriberInitializer
            var backgroundSubscriberInitializer = new BackgroundSubscriberInitializer(app.Lifetime, app.Services);
            await backgroundSubscriberInitializer.StartBackgroundSubscribers();
 
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

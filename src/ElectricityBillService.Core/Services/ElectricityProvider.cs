using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityBillService.Infrastructure.Provider
{ 
    public interface IElectricityProvider
    {
        Task<bool> ProcessBillPaymentAsync(object bill);
    }
       

    public class MockProviderA : IElectricityProvider
    {
        public Task<bool> ProcessBillPaymentAsync(dynamic bill)
        {
            Console.WriteLine($"Processing payment with Provider A for Bill: {bill.Id}");
            return Task.FromResult(true); // Simulated success
        }
    }

    public class MockProviderB : IElectricityProvider
    {
        public Task<bool> ProcessBillPaymentAsync(dynamic bill)
        {
            Console.WriteLine($"Processing payment with Provider B for Bill: {bill.Id}");
            return Task.FromResult(true); // Simulated success
        }
    }

    public class ProviderSelectorService : IElectricityProvider
    {
        private readonly IElectricityProvider _providerA;
        private readonly IElectricityProvider _providerB;

        public ProviderSelectorService(MockProviderA providerA, MockProviderB providerB)
        {
            _providerA = providerA;
            _providerB = providerB;
        }

        public Task<bool> ProcessBillPaymentAsync(object bill)
        {
            // Randomly choose a provider for simulation purposes
            var selectedProvider = new Random().Next(2) == 0 ? _providerA : _providerB;
            return selectedProvider.ProcessBillPaymentAsync(bill);
        }
    }
}

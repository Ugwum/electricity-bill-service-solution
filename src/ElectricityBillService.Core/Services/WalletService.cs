using ElectricityBillService.Core.EventHandling.Events;
using ElectricityBillService.Core.Interfaces;
using ElectricityBillService.Core.Models;
using ElectricityBillService.DataAccess.Repositories;
using ElectricityBillService.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ElectricityBillService.Core.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IConfiguration _config;
        private readonly ISmsService _smsService;
        private readonly IEventPublisher _eventPublisher;
        public WalletService(IWalletRepository walletRepository, IConfiguration config, IEventPublisher eventPublisher)
        {
            _walletRepository = walletRepository;
            _eventPublisher = eventPublisher;
        }
        public async Task<APIResult> AddFundsAsyn(Guid walletId, decimal amount)
        {
            var wallet = await _walletRepository.GetWalletByIdAsync(walletId.ToString());

            if(wallet is null) { return APIResult.Failure($"Invalid wallet ID {walletId}"); }

            wallet.Balance += amount;
            _walletRepository.UpdateAsync(wallet);


            try { await _eventPublisher.PublishAsync(new WalletFundAddedEvent(walletId, amount)); } catch { };


            return APIResult.Success(new { walletID = wallet.Id, balance = wallet.Balance}, $"fund wallet successful for walletID {walletId}");
 
        }
    }
}

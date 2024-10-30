using ElectricityBillService.Core.EventHandling.Events;
using ElectricityBillService.Core.Interfaces;
using ElectricityBillService.Core.Models;
using ElectricityBillService.DataAccess.Repositories;
using ElectricityBillService.Infrastructure;

namespace ElectricityBillService.Core.Services
{
    public class BillService : IBillService
    {
        private readonly IEventPublisher _eventPublisher;
       
        private readonly IBillRepository _billRepository;
        private readonly ISmsService _smsService;
        private readonly IWalletRepository _walletRepository;
        public BillService(IEventPublisher eventPublisher, IWalletRepository walletRepository, 
            IBillRepository billRepository, ISmsService smsService)
        {
            _eventPublisher = eventPublisher;
            _walletRepository = _walletRepository;
            _billRepository = billRepository;
            _smsService = smsService;
            
        }

        public async Task<APIResult> CreateBillAsync(decimal amount)
        {
            var bill = new Bill { Id = Guid.NewGuid(), Amount = amount };
            _billRepository.CreateBillAsync(bill);

            await _eventPublisher.PublishAsync(new BillCreatedEvent(bill.Id.ToString(), amount));

            return APIResult.Success(bill, $"Bill payment for reference {bill.Id} is successfull"); 
        } 
        public async Task<APIResult> ProcessPaymentAsync(string validationRef, string walletId)
        {
            try
            {
                var bill = await _billRepository.GetByValidationRefAsync(validationRef);
                if (bill == null)
                {
                    return APIResult.Failure("Bill not found.");
                }

                if (bill.Status == BillStatus.Paid)
                {
                    return APIResult.Failure("Bill already paid.");
                }

                var wallet = await _walletRepository.GetWalletByIdAsync(walletId);
                if (wallet == null)
                {
                    return APIResult.Failure("Wallet not found.");
                }

                if (wallet.Balance < bill.Amount)
                {
                    return APIResult.Failure("Insufficient wallet balance.");
                }

                // Charge the wallet
                var chargeWalletSuccessful = await _walletRepository.ChargeWalletAsync(wallet.Id, bill.Amount);

                if (!chargeWalletSuccessful) { return APIResult.Failure("An error occurred: Unable to charge wallet."); }

                // Update bill status
                bill.Status = BillStatus.Paid;
                bill.WalletId = wallet.Id.ToString();
                await _billRepository.UpdateAsync(bill);

                // Publish event for payment completed
                try { await _eventPublisher.PublishAsync(new PaymentCompletedEvent(bill.Id.ToString(), walletId,  bill.Amount)); } catch { };

                return APIResult.Success(bill, $"Bill payment for reference {bill.Id} is successfull");
            }
            catch(Exception ex)
            {
                return APIResult.Failure($"An error occurred. {ex.Message}");
            }
        } 
    }
}

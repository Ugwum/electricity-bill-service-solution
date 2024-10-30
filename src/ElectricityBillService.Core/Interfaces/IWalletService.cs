using ElectricityBillService.Core.Models;

namespace ElectricityBillService.Core.Interfaces
{
    public interface IWalletService
    {
        Task<APIResult> AddFundsAsyn(Guid walletId, decimal amount);     

    }
}

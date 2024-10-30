using ElectricityBillService.Core.Models;

namespace ElectricityBillService.DataAccess.Repositories
{
    public interface IWalletRepository
    { 
        Task<Wallet> GetWalletByIdAsync(string walletId);

        Task<bool> ChargeWalletAsync(Guid walletId, decimal amount);

        Task UpdateAsync(Wallet wallet);
    }
}

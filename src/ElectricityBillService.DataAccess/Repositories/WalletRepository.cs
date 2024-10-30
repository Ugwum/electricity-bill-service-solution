using ElectricityBillService.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityBillService.DataAccess.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ElectricityBillDBContext _context;

        public WalletRepository(ElectricityBillDBContext context)
        {
            _context = context;
        }

        public async Task<bool> ChargeWalletAsync(Guid walletId, decimal amount)
        {
            var wallet = await GetWalletByIdAsync(walletId.ToString());
            if (wallet != null && wallet.Balance >= amount)
            {
                wallet.Balance -= amount;
                await UpdateAsync(wallet);
                return true;
            }
            return false;
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task<Wallet> GetWalletByIdAsync(string walletId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.Id.ToString() == walletId);
        }
    }

}

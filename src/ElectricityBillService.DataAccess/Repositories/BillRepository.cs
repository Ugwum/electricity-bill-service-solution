using ElectricityBillService.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectricityBillService.DataAccess.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly ElectricityBillDBContext _context;

        public BillRepository(ElectricityBillDBContext context)
        {
            _context = context;
        }

        public async Task CreateBillAsync(Bill bill)
        {
            await _context.Bills.AddAsync(bill);
            await _context.SaveChangesAsync();
        }

        public async Task<Bill> GetByValidationRefAsync(string validationRef)
        {
            return await _context.Bills.SingleOrDefaultAsync(b => b.Id.ToString() == validationRef);
        }

        public async Task UpdateAsync(Bill bill)
        {
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync();
        }
    }
}

using ElectricityBillService.Core.Models;

namespace ElectricityBillService.DataAccess.Repositories
{
    public interface IBillRepository
    {
        Task CreateBillAsync(Bill bill);
        Task<Bill> GetByValidationRefAsync(string validationRef);
        Task UpdateAsync(Bill bill);
    }
}

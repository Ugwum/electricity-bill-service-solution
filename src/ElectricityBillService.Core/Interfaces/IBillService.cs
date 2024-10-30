using ElectricityBillService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityBillService.Core.Interfaces
{
    public interface IBillService
    {
        Task<APIResult> CreateBillAsync(decimal amount);
      
        Task<APIResult> ProcessPaymentAsync(string validationRef, string walletId);
    }
}

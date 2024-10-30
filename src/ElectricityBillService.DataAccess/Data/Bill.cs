using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityBillService.Core.Models
{
    public class Bill
    { 
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; } = BillStatus.Pending;

        public string WalletId { get; set; }
         
    }

    public enum BillStatus
    {
        Pending = 1,
        Paid = 2, Unlocked = 3,
    }
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityBillService.Core.Models
{
    public class Wallet
    {
        public Guid Id { get; set; }

        public decimal Balance { get; set; }

        public string PhoneNumber { get; set; }
    }
}

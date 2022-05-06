using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Models
{
    public class NanoPaymentData
    {
        public string ReceiveAddress { get; set; }
        public byte[] ReceivePrivateKey { get; set; }
        public decimal MinimumAmount { get; set; }
        public DateTime MinimumAmountExpireDate { get; set; }
        public string PayerAddress { get; set; }
        public decimal? PaidAmount { get; set; }
        public string PaymentBlock { get; set; }
        public DateTime ReceiveDate { get; set; }
    }
}

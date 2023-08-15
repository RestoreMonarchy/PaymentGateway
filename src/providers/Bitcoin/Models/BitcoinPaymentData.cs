using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models
{
    public class BitcoinPaymentData
    {
        public string ReceiveAddress { get; set; }
        public byte[] ReceivePrivateKey { get; set; }
        public string SendAddress { get; set; }
        public decimal MinimumAmount { get; set; }
        public string PayerAddress { get; set; }
        public decimal? PaidAmount { get; set; }
        public string PaymentHash { get; set; }
        public DateTime ReceiveDate { get; set; }
    }
}

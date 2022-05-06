using Nano.Net;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Models
{
    public class NanoTransaction
    {
        public string ReceiveAddress { get; set; }
        public string SendAddress { get; set; }
        public string AmountRaw { get; set; }
        public string BlockHash { get; set; }
        public bool IsReceiveable { get; set; }

        public Amount Amount => Amount.FromRaw(AmountRaw);
        public decimal RoundedAmount => NumbersHelper.RoundNano(Amount.Nano);
    }
}

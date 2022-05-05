using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Models
{
    public class WaitingNanoPayment
    {
        public Guid PublicId { get; set; }
        public string ReceiveAddress { get; set; }
    }
}

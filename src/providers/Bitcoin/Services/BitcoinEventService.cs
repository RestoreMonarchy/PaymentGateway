using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Services
{
    public class BitcoinEventService
    {
        public event Action<Guid> OnPaymentReceived;
        public void TriggerOnPaymentReceived(Guid paymentId)
        {
            OnPaymentReceived?.Invoke(paymentId);
        }
    }
}

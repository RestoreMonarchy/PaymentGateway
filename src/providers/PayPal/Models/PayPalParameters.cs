using RestoreMonarchy.PaymentGateway.API.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.PayPal.Models
{
    public class PayPalParameters : IPaymentProviderParameters
    {
        public string DefaultReceiver { get; set; }
        public bool UseSandbox { get; set; }

        public string GetUrl()
        {
            if (UseSandbox)
                return "https://www.sandbox.paypal.com/cgi-bin/";
            else
                return "https://www.paypal.com/cgi-bin/";
        }

        public string GetReceiver(string receiver)
        {
            if (string.IsNullOrEmpty(receiver))
                return DefaultReceiver;
            else
                return receiver;
        }
    }
}

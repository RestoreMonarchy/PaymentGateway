using Microsoft.AspNetCore.Components;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Components
{
    public partial class NanoPayment
    {
        [Parameter]
        public PaymentInfo PaymentInfo { get; set; }

        public NanoPaymentData Data { get; set; }

        protected override void OnParametersSet()
        {
            Data = PaymentInfo.Data.GetObject<NanoPaymentData>();
        }
    }
}

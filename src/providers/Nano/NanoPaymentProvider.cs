using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.API.Results;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Components;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Services;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano
{
    public class NanoPaymentProvider : PaymentProvider
    {
        public override string Name => "Nano";

        public override Type FormComponentType => null;

        public override Type InfoComponentType => null;

        private readonly NanoPaymentStore paymentStore;

        public NanoPaymentProvider(NanoPaymentStore paymentStore)
        {
            this.paymentStore = paymentStore;
        }

        public override async Task<UserAction> StartPaymentAsync(Guid publicId)
        {
            await paymentStore.GetOrCreateNanoPaymentAsync(publicId);
            return RazorComponent(typeof(NanoPayment));
        }
    }
}

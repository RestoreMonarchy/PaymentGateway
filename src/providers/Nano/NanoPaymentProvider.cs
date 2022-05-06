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
        public override Type InfoComponentType => typeof(NanoPaymentInfo);

        private readonly NanoService nanoService;

        public NanoPaymentProvider(NanoService nanoService)
        {
            this.nanoService = nanoService;
        }

        public override async Task<UserAction> StartPaymentAsync(Guid publicId)
        {
            await nanoService.GetOrCreatePaymentAsync(publicId);
            return RazorComponent(typeof(NanoPayment));
        }
    }
}

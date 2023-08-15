using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.API.Results;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Components;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Services;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin
{
    public class BitcoinPaymentProvider : PaymentProvider
    {
        public override string Name => "Bitcoin";

        public override Type FormComponentType => null;

        public override Type InfoComponentType => typeof(BitcoinPaymentInfo);

        private readonly BitcoinService _bitcoinService;

        public BitcoinPaymentProvider(BitcoinService bitcoinService)
        {
            _bitcoinService = bitcoinService;
        }

        public override async Task<UserAction> StartPaymentAsync(Guid publicId)
        {
            await _bitcoinService.CreatePaymentAsync(publicId);
            return RazorComponent(typeof(BitcoinPayment));
        }
    }
}

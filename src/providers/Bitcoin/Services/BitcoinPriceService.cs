using Microsoft.Extensions.Options;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Clients;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Helpers;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Services
{
    public class BitcoinPriceService
    {
        private readonly CoinMarketCapClient _client;
        private readonly BitcoinOptions _options;
        private readonly IPaymentService _paymentService;

        public BitcoinPriceService(CoinMarketCapClient client, IOptions<BitcoinOptions> options, IPaymentService paymentService)
        {
            _client = client;
            _options = options.Value;
            _paymentService = paymentService;
        }

        public async Task SetMinimumAmountAsync(PaymentInfo payment)
        {
            decimal amount = await _client.ConvertToAsync(payment.Amount, payment.Currency, "BTC");

            BitcoinPaymentData paymentData = payment.Data.GetObject<BitcoinPaymentData>();
            paymentData.MinimumAmount = NumbersHelper.RoundBitcoin(amount);

            payment.Data.UpdateObject(paymentData);
        }
    }
}

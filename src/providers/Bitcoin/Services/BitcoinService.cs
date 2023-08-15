using Microsoft.Extensions.Logging;
using NBitcoin;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Services
{
    public class BitcoinService
    {
        private readonly IPaymentService _paymentService;
        private readonly WaitingBitcoinPaymentStore _paymentStore;
        private readonly BitcoinPriceService _priceService;
        private readonly ILogger<BitcoinService> _logger;

        public BitcoinService(
            BitcoinPriceService priceService,
            WaitingBitcoinPaymentStore paymentStore,
            IPaymentService paymentService,
            ILogger<BitcoinService> logger)
        {
            _priceService = priceService;
            _paymentStore = paymentStore;
            _paymentService = paymentService;
            _logger = logger;
        }


        public async Task CreatePaymentAsync(Guid publicId)
        {
            _logger.LogDebug("Creating btc payment for {publicId}", publicId);
            var payment = await _paymentService.GetPaymentInfo(publicId);
            if (payment is null)
            {
                _logger.LogError("Payment {publicId} was not found", publicId);
                return;
            }

            if (payment.Data.GetObject<BitcoinPaymentData>() is not null) return; // already initialized

            _logger.LogDebug("Generating gateway btc wallet");
            Key privateKey = new();
            var paymentData = new BitcoinPaymentData
            {
                ReceiveAddress = privateKey.GetAddress(ScriptPubKeyType.Segwit, Network.Main).ToString(),
                ReceivePrivateKey = privateKey.ToBytes(),
            };
            _logger.LogDebug("Updating payment data");
            payment.Data.UpdateObject(paymentData);
            await _priceService.SetMinimumAmountAsync(payment);
            await _paymentService.UpdatePaymentData(publicId, paymentData);
            _logger.LogDebug("Adding payment to waiting");
            _paymentStore.Add(new WaitingBitcoinPayment
            {
                PublicId = payment.PublicId,
                ReceiveAddress = paymentData.ReceiveAddress
            });

        }
    }
}

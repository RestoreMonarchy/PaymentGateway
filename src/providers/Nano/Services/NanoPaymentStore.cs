using Microsoft.Extensions.Options;
using Nano.Net;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Clients;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Helpers;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Services
{
    public class NanoPaymentStore
    {
        private readonly IPaymentService paymentService;
        private readonly CoinMarketCapClient coinMarketCapClient;
        private readonly NanoOptions options;

        public IEnumerable<PaymentInfo> PendingPayments => pendingPayments;
        public string[] PendingPaymentsAddress => pendingPayments.Select(x => x.Data.GetObject<NanoPaymentData>().ReceiveAddress).ToArray();

        private List<PaymentInfo> pendingPayments;
        public delegate void PendingPaymentsUpdated();
        public event PendingPaymentsUpdated OnPendingPaymentsUpdated;
        private void TriggerOnPendingPaymentsUpdated() => OnPendingPaymentsUpdated?.Invoke();

        public NanoPaymentStore(IPaymentService paymentService, CoinMarketCapClient coinMarketCapClient, IOptions<NanoOptions> options)
        {
            this.paymentService = paymentService;
            this.coinMarketCapClient = coinMarketCapClient;
            this.options = options.Value;

            pendingPayments = new List<PaymentInfo>();
        }

        public async Task ReceivePaymentAsync(PaymentInfo payment, string payerAddress, decimal amount)
        {
            NanoPaymentData paymentData = payment.Data.GetObject<NanoPaymentData>();
            paymentData.PayerAddress = payerAddress;
            paymentData.PaidAmount = amount;
            await paymentService.UpdatePaymentData(payment.PublicId, paymentData);
            pendingPayments.RemoveAll(x => x.PublicId == payment.PublicId);
            TriggerOnPendingPaymentsUpdated();
        }

        public async Task ReloadPendingPaymentsAsync()
        {
            pendingPayments = (await paymentService.GetPendingPayments("Nano")).ToList();
        }

        public async Task<NanoPaymentData> GetOrCreateNanoPaymentAsync(Guid publicId)
        {
            PaymentInfo payment = await paymentService.GetPaymentInfo(publicId);
            
            NanoPaymentData paymentData = payment.Data.GetObject<NanoPaymentData>();

            if (paymentData != null)
            {
                await TryUpdateMinimumAmountAsync(payment);
                return paymentData;
            }

            Account account = Account.FromSeed(Utils.GenerateSeed(), 0);
            paymentData = new()
            {
                ReceiveAddress = account.Address,
                ReceivePrivateKey = account.PrivateKey
            };
            payment.Data.UpdateObject(paymentData);

            await SetMinimumAmountAsync(payment);
            await paymentService.UpdatePaymentData(payment.PublicId, paymentData);

            pendingPayments.Add(payment);
            TriggerOnPendingPaymentsUpdated();

            return paymentData;
        }

        public bool IsPending(string address)
        {
            return pendingPayments.Exists(x => x.Data.GetObject<NanoPaymentData>().ReceiveAddress == address);
        }

        public PaymentInfo GetPendingPaymentByReceiver(string receiveAddress)
        {
            return pendingPayments.FirstOrDefault(x => x.Data.GetObject<NanoPaymentData>().ReceiveAddress == receiveAddress);
        }

        public async Task TryUpdateMinimumAmountAsync(PaymentInfo payment)
        {
            if (payment.Data.GetObject<NanoPaymentData>().MinimumAmountExpireDate >= DateTime.Now)
            {
                return;
            }

            await UpdateMinimumAmountAsync(payment);
        }

        private async Task SetMinimumAmountAsync(PaymentInfo payment)
        {
            decimal amount = await coinMarketCapClient.ConvertToAsync(payment.Amount, payment.Currency, "XNO");
            NanoPaymentData paymentData = payment.Data.GetObject<NanoPaymentData>();

            paymentData.MinimumAmount = NumbersHelper.RoundNano(amount);
            paymentData.MinimumAmountExpireDate = DateTime.Now.AddMinutes(options.PaymentExpireMinutes);
        }

        public async Task UpdateMinimumAmountAsync(PaymentInfo payment)
        {
            await SetMinimumAmountAsync(payment);
            await paymentService.UpdatePaymentData(payment.PublicId, payment.Data.GetObject<NanoPaymentData>());
        }
    }
}

using Nano.Net;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Services
{
    public class NanoService
    {
        private readonly WaitingNanoPaymentStore paymentStore;
        private readonly NanoPriceService priceService;
        private readonly IPaymentService paymentService;

        public NanoService(WaitingNanoPaymentStore paymentStore, NanoPriceService priceService, IPaymentService paymentService)
        {
            this.paymentStore = paymentStore;
            this.priceService = priceService;
            this.paymentService = paymentService;
        }

        public async ValueTask<NanoPaymentData> GetOrCreatePaymentAsync(Guid publicId)
        {
            PaymentInfo payment = await paymentService.GetPaymentInfo(publicId);

            NanoPaymentData paymentData = payment.Data.GetObject<NanoPaymentData>();

            if (paymentData != null)
            {
                await priceService.ValidateMinimumAmountAsync(payment);
                return paymentData;
            }

            return await InitializePaymentAsync(payment);
        }

        private async ValueTask<NanoPaymentData> InitializePaymentAsync(PaymentInfo payment)
        {
            Account account = new(Utils.GenerateSeed(), 0);
            
            NanoPaymentData paymentData = new()
            {
                ReceiveAddress = account.Address,
                ReceivePrivateKey = account.PrivateKey
            };
            payment.Data.UpdateObject(paymentData);

            await priceService.SetMinimumAmountAsync(payment);

            await paymentService.UpdatePaymentData(payment.PublicId, paymentData);

            WaitingNanoPayment waitingPayment = new()
            {
                PublicId = payment.PublicId,
                ReceiveAddress = paymentData.ReceiveAddress
            };

            paymentStore.Add(waitingPayment);
            return paymentData;
        }
    }
}

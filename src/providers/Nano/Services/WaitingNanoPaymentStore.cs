using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Constants;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Services
{
    public class WaitingNanoPaymentStore
    {
        private readonly IPaymentService paymentService;

        public WaitingNanoPaymentStore(IPaymentService paymentService)
        {
            waitingPayments = new();
            this.paymentService = paymentService;
        }

        private List<WaitingNanoPayment> waitingPayments;

        public event Action<WaitingNanoPayment> OnUpdate;
        private void TriggerOnUpdate(WaitingNanoPayment payment)
        {
            OnUpdate?.Invoke(payment);
        }

        public async ValueTask ReloadAsync()
        {
            IEnumerable<PaymentInfo> pendingPayments = await paymentService.GetPendingPayments(NanoConstants.ProviderName);

            waitingPayments.Clear();
            foreach (PaymentInfo paymentInfo in pendingPayments)
            {
                NanoPaymentData data = paymentInfo.Data.GetObject<NanoPaymentData>();
                // TODO: data should never be null for nano payment, it should be generated and inserted when creating a payment
                if (data == null)
                    continue;
                WaitingNanoPayment waitingPayment = new()
                {
                    PublicId = paymentInfo.PublicId,
                    ReceiveAddress = data.ReceiveAddress
                };

                waitingPayments.Add(waitingPayment);
            }
        }

        public bool RemoveByPublicId(Guid publicId)
        {
            return waitingPayments.RemoveAll(x => x.PublicId == publicId) > 0;
        }

        public void Add(WaitingNanoPayment payment)
        {
            waitingPayments.Add(payment);
            TriggerOnUpdate(payment);
        }

        public bool TryGetByReceiveAddress(string address, out WaitingNanoPayment payment)
        {
            payment = GetByReceiveAddress(address);
            return payment != null;
        }

        public string[] GetAllAddresses()
        {
            return waitingPayments.Select(x => x.ReceiveAddress).ToArray();
        }

        public WaitingNanoPayment GetByReceiveAddress(string address)
        {
            return waitingPayments.FirstOrDefault(x => x.ReceiveAddress == address);
        }
    }
}

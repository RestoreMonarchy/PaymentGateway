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

        public event Action<WaitingNanoPayment> OnWaitingPaymentAdded;
        private void TriggerOnWaitingPaymentAdded(WaitingNanoPayment payment)
        {
            OnWaitingPaymentAdded?.Invoke(payment);
        }

        public async ValueTask ReloadAsync()
        {
            IEnumerable<PaymentInfo> pendingPayments = await paymentService.GetPendingPayments(NanoConstants.ProviderName);

            waitingPayments.Clear();
            foreach (PaymentInfo paymentInfo in pendingPayments)
            {
                NanoPaymentData data = paymentInfo.Data.GetObject<NanoPaymentData>();
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
            TriggerOnWaitingPaymentAdded(payment);
        }

        public IEnumerable<string> GetAllAddresses()
        {
            return waitingPayments.Select(x => x.ReceiveAddress);
        }

        public WaitingNanoPayment GetByReceiveAddress(string address)
        {
            return waitingPayments.FirstOrDefault(x => x.ReceiveAddress == address);
        }
    }
}

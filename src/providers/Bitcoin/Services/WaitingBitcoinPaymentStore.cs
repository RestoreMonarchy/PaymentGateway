using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Services
{
    public class WaitingBitcoinPaymentStore
    {
        private readonly IPaymentService _paymentService;

        public WaitingBitcoinPaymentStore(IPaymentService paymentService)
        {
            _waitingPayments = new();
            _paymentService = paymentService;
        }

        private List<WaitingBitcoinPayment> _waitingPayments;

        public async ValueTask ReloadAsync()
        {
            IEnumerable<PaymentInfo> pendingPayments = await _paymentService.GetPendingPayments("Bitcoin");

            _waitingPayments.Clear();
            foreach (PaymentInfo paymentInfo in pendingPayments)
            {
                BitcoinPaymentData data = paymentInfo.Data.GetObject<BitcoinPaymentData>();
                if (data == null)
                    continue;

                WaitingBitcoinPayment waitingPayment = new()
                {
                    PublicId = paymentInfo.PublicId,
                    ReceiveAddress = data.ReceiveAddress
                };

                _waitingPayments.Add(waitingPayment);
            }
        }

        public bool RemoveByPublicId(Guid publicId)
        {
            return _waitingPayments.RemoveAll(x => x.PublicId == publicId) > 0;
        }

        public void Add(WaitingBitcoinPayment payment)
        {
            _waitingPayments.Add(payment);
        }

        public bool TryGetByReceiveAddress(string address, out WaitingBitcoinPayment payment)
        {
            payment = GetByReceiveAddress(address);
            return payment != null;
        }

        public WaitingBitcoinPayment GetByReceiveAddress(string address)
        {
            return _waitingPayments.Find(x => x.ReceiveAddress == address);
        }
    }
}

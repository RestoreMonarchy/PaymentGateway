namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Services
{
    public class NanoEventService
    {
        public event Action<Guid> OnPaymentReceived;
        public void TriggerOnPaymentReceived(Guid paymentId)
        {
            OnPaymentReceived?.Invoke(paymentId);
        }
    }
}

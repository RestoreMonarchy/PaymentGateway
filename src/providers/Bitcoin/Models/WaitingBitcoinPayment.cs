namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models
{
    public class WaitingBitcoinPayment
    {
        public Guid PublicId { get; set; }
        public string ReceiveAddress { get; set; }
    }
}

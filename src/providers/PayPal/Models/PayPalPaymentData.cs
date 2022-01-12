namespace RestoreMonarchy.PaymentGateway.Providers.PayPal.Models
{
    public class PayPalPaymentData
    {
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string PayerEmail { get; set; }
        public decimal Gross { get; set; }
        public decimal Fee { get; set; }
        public string Currency { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

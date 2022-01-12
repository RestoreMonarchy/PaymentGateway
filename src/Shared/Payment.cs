namespace RestoreMonarchy.PaymentGateway.Shared
{
    public class Payment
    {
        public string Provider { get; set; }        
        public string Custom { get; set; }
        public string Receiver { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }

        public IEnumerable<PaymentItem> Items { get; set; }
    }
}
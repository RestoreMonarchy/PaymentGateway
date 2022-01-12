namespace RestoreMonarchy.PaymentGateway.API.Models
{
    public class PaymentInfo
    {
        public Guid PublicId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Receiver { get; set; }
        public JsonData Data { get; set; }
        public bool IsCompleted { get; set; }

        public StoreInfo Store { get; set; }
        public List<PaymentItemInfo> Items { get; set; }
    }
}

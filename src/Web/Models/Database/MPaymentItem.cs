namespace RestoreMonarchy.PaymentGateway.Web.Models.Database
{
    public class MPaymentItem
    {
        public int PaymentId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

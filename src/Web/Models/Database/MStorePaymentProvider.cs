namespace RestoreMonarchy.PaymentGateway.Web.Models.Database
{
    public class MStorePaymentProvider
    {
        public int StoreId { get; set; }
        public string PaymentProvider { get; set; }
        public string JsonParameters { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreateDate { get; set; }

        public MStorePaymentProvider Clone()
        {
            return new MStorePaymentProvider()
            {
                StoreId = StoreId,
                PaymentProvider = PaymentProvider,
                JsonParameters = JsonParameters,
                IsEnabled = IsEnabled,
                CreateDate = CreateDate
            };
        }
    }
}

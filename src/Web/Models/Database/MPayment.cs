using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.Client.Models;

namespace RestoreMonarchy.PaymentGateway.Web.Models.Database
{
    public class MPayment
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public int StoreId { get; set; }
        public string Provider { get; set; }
        public string Custom { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Receiver { get; set; }
        public string JsonData { get; set; }        
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsNotified { get; set; }
        public DateTime? NotifiedDate { get; set; }
        public byte NotifiedCount { get; set; }
        public DateTime CreateDate { get; set; }

        public List<MPaymentItem> Items { get; set; }
        public MStore Store { get; set; }

        public static MPayment FromPayment(Payment payment, int storeId)
        {
            MPayment result = new()
            {
                StoreId = storeId,
                Provider = payment.Provider,
                Custom = payment.Custom,
                Currency = payment.Currency,
                Receiver = payment.Receiver,
                Amount = payment.Amount,
                Items = new List<MPaymentItem>()
            };

            foreach (var item in payment.Items)
            {
                result.Items.Add(new MPaymentItem()
                {
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            return result;
        }

        public PaymentInfo ToPaymentInfo()
        {
            PaymentInfo info = new PaymentInfo()
            {
                PublicId = PublicId,
                Receiver = Receiver,
                Amount = Amount,
                Currency = Currency,
                Data = new JsonData(JsonData),
                IsCompleted = IsCompleted,
                Store = new StoreInfo()
                {
                    Name = Store.Name,
                    LogoUrl = Store.LogoUrl,
                    BackgroundUrl = Store.BackgroundUrl,                    
                    CancelUrl = Store.DefaultCancelUrl,
                    ReturnUrl = Store.DefaultReturnUrl
                },
                Items = new List<PaymentItemInfo>()
            };

            foreach (MPaymentItem item in Items)
            {
                info.Items.Add(new PaymentItemInfo()
                {
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity
                });
            }

            return info;
        }
    }
}

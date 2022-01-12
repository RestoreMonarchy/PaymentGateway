namespace RestoreMonarchy.PaymentGateway.Client.Models
{
    public class Payment
    {
        public string Provider { get; set; }
        public string Custom { get; set; }
        public string Receiver { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }

        public List<PaymentItem> Items { get; set; }

        public static Payment Create(string provider, string custom, string receiver, string currency, decimal amount)
        {
            return new Payment() 
            {
                Provider = provider,
                Custom = custom,
                Receiver = receiver,
                Currency = currency,
                Amount = amount,
                Items = new List<PaymentItem>()
            };
        }

        public void AddItem(string name, int quantity, decimal price)
        {
            if (Items == null)
                Items = new List<PaymentItem>();

            Items.Add(new PaymentItem()
            {
                Name = name,
                Quantity = quantity,
                Price = price
            });
        }

        public decimal SumItemsAmount()
        {
            if (Items == null)
            {
                return 0;
            }

            return Items.Sum(x => x.Quantity * x.Price);
        }

        public bool ValidateAmount() => SumItemsAmount() == Amount;
    }
}

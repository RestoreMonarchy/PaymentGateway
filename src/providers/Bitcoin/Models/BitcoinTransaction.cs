namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models
{
    public class BitcoinTransaction
    {
        public string SendAddress { get; set; }
        public List<(string ReceiveAddress, decimal Amount)> Outputs { get; set; }
        public string TransactionHash { get; set; }
    }
}

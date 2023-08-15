namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models
{
    public class BitcoinOptions
    {
        public string NodeAddress { get; set; }

        public string CoinMarketCapAPIUrl { get; set; }
        public string CoinMarketCapAPIKey { get; set; }
    }
}

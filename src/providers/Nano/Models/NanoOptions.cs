using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Models
{
    public class NanoOptions
    {
        public const string Key = "Nano";

        public string NodeUrl { get; set; }
        public string WebSocketUrl { get; set; }
        
        public string CoinMarketCapAPIUrl { get; set; }
        public string CoinMarketCapAPIKey { get; set; }
        public double PriceRefreshMinutes { get; set; }
    }
}

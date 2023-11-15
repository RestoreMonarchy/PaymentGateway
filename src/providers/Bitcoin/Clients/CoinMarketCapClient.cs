using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Web;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models;
using System.Globalization;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Clients
{
    public class CoinMarketCapClient
    {
        private readonly BitcoinOptions options;
        private readonly IHttpClientFactory httpClientFactory;

        public CoinMarketCapClient(IOptions<BitcoinOptions> options, IHttpClientFactory httpClientFactory)
        {
            this.options = options.Value;
            this.httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient()
        {
            HttpClient client = httpClientFactory.CreateClient("CoinMarketCap");
            client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", options.CoinMarketCapAPIKey);
            client.BaseAddress = new Uri(options.CoinMarketCapAPIUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
            return client;
        }

        public async Task<decimal> ConvertToAsync(decimal amount, string currencyFrom, string currencyTo)
        {
            HttpClient client = CreateClient();

            NameValueCollection collection = HttpUtility.ParseQueryString(string.Empty);
            collection["amount"] = amount.ToString(CultureInfo.GetCultureInfo("en-US"));
            collection["symbol"] = currencyFrom;
            collection["convert"] = currencyTo;

            string responseJson = await client.GetStringAsync($"v1/tools/price-conversion?{collection.ToString()}");
            JObject obj = (JObject)JsonConvert.DeserializeObject(responseJson);
            return (decimal)obj["data"]["quote"][currencyTo]["price"];
        }
    }
}

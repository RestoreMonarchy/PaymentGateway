using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;
using System.Collections.Specialized;
using System.Web;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Clients
{
    public class CoinMarketCapClient
    {
        private readonly NanoOptions options;
        private readonly IHttpClientFactory httpClientFactory;

        public CoinMarketCapClient(IOptions<NanoOptions> options, IHttpClientFactory httpClientFactory)
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
            collection["amount"] = amount.ToString();
            collection["symbol"] = currencyFrom;
            collection["convert"] = currencyTo;

            string responseJson = await client.GetStringAsync($"v1/tools/price-conversion?{collection.ToString()}");
            JObject obj = (JObject)JsonConvert.DeserializeObject(responseJson);
            return (decimal)obj["data"]["quote"][currencyTo]["price"];
        }
    }
}

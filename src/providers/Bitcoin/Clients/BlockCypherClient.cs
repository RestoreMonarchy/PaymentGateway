using NBitcoin;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Clients
{
    public class BlockCypherClient
    {
        public async Task<List<Coin>> GetUnSpentCoinsAsync(string address)
        {
            using var client = new HttpClient();
            var response = await client.GetFromJsonAsync<BlockCypherFullAddressResponse>($"https://api.blockcypher.com/v1/btc/main/addrs/{address}/full");
            var spentTransactionHashes = new List<string>();
            var receivedTransactions = new List<(string Address, string Hash, string Script, int Sats, int Index)>();
            foreach (var transaction in response.Txs)
            {
                var spentTransaction = transaction.Inputs.Where(i => i.Addresses.Contains(address));
                if (spentTransaction.Any())
                {
                    spentTransactionHashes.AddRange(spentTransaction.Select(t => t.PrevHash));
                }

                var receivedTransaction = transaction.Outputs.Where(o => o.Addresses.Contains(address));
                if (receivedTransaction.Any())
                {
                    var output = receivedTransaction.FirstOrDefault(o => o.Addresses.FirstOrDefault() == address);
                    var index = transaction.Outputs.FindIndex(a => a.Addresses.FirstOrDefault() == address);
                    receivedTransactions.Add((address, transaction.Hash, output.Script, output.Value, index));
                }
            }

            return receivedTransactions.Where(t => !spentTransactionHashes.Contains(t.Hash))
                .Select(t => new Coin(uint256.Parse(t.Hash), uint.Parse(t.Index.ToString()), new Money(t.Sats, MoneyUnit.Satoshi), Script.FromHex(t.Script)))
                .ToList();
        }

        /// <summary>
        /// Gets average fees from btc blockchain
        /// </summary>
        /// <returns>Expressed in sat/b</returns>
        public async Task<FeeRate> GetAverageFeesAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync("https://api.blockcypher.com/v1/btc/main");
            var obj = JsonObject.Parse(response);
            var statoshiPerByte = obj["medium_fee_per_kb"].GetValue<decimal>() / 1000;
            return new FeeRate(statoshiPerByte);
        }
    }
}

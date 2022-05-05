using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.Net;
using Nano.Net.Extensions;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;
    
namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Clients
{
    public class NanoNodeClient
    {
        private readonly NanoOptions options;
        private readonly ILogger<NanoNodeClient> logger;

        private readonly RpcClient rpcClient;

        public NanoNodeClient(IOptions<NanoOptions> options, ILogger<NanoNodeClient> logger)
        {
            this.options = options.Value;
            this.logger = logger;

            rpcClient = new RpcClient(this.options.NodeUrl);
        }

        public Account CreateAccount(byte[] privateKey)
        {
            return new Account(privateKey);
        }

        public async Task<Dictionary<string, Dictionary<string, ReceivableBlock>>> GetAccountsPendingAsync(string[] accounts)
        {
            AccountsPendingResponse response = await rpcClient.AccountsPendingAsync(accounts, -1);
            return response.Blocks;
        }

        public async Task<HistoryBlock[]> GetAccountHistoryAsync(string address)
        {
            AccountHistoryResponse response = await rpcClient.AccountHistoryAsync(address, -1);
            return response.History ?? Array.Empty<HistoryBlock>();
        }

        public async Task<Dictionary<string, BalancePendingPair>> GetAccountsBalancesAsync(string[] accounts)
        {
            AccountsBalancesResponse response = await rpcClient.AccountsBalancesAsync(accounts);
            return response.Balances ?? new Dictionary<string, BalancePendingPair>();
        }

        public async Task SendBalanceAsync(Account account, string receiveAddress)
        {
            await UpdateAccountAsync(account);
            await ProcessSendBlockAsync(account, receiveAddress, account.Balance);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            await rpcClient.UpdateAccountAsync(account);
        }

        public async Task ReceiveBlockAsync(Account account, string blockHash, Amount amount)
        {
            await UpdateAccountAsync(account);
            string pow = await GetPowAsync(account);

            Block receiveBlock = Block.CreateReceiveBlock(account, blockHash, amount, pow);
            await rpcClient.ProcessAsync(receiveBlock);
        }

        public async Task SendBlockAsync(Account account, string receiveAddress, Amount amount)
        {
            await UpdateAccountAsync(account);
            await ProcessSendBlockAsync(account, receiveAddress, amount);
        }

        private async Task ProcessSendBlockAsync(Account account, string receiveAddress, Amount amount)
        {
            string pow = await GetPowAsync(account);

            Block sendBlock = Block.CreateSendBlock(account, receiveAddress, amount, pow);
            await rpcClient.ProcessAsync(sendBlock);
        }
            
        private async Task<string> GetPowAsync(Account account)
        {
            WorkGenerateResponse workGen;

            string hash;
            if (!account.Opened)
            {
                hash = account.PublicKey.BytesToHex();
            } else
            {
                hash = account.Frontier;                
            }

            workGen = await rpcClient.WorkGenerateAsync(hash);
            return workGen.Work;
        }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.Net;
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
            return Account.FromPrivateKey(privateKey);
        }

        public async Task<Dictionary<string, Dictionary<string, ReceivableBlock>>> GetAccountsPendingAsync(string[] accounts)
        {
            AccountsPendingResponse response = await rpcClient.AccountsPendingAsync(accounts);
            return response.Blocks;
        }

        public async Task ReceiveBlockAsync(Account account, string blockHash, Amount amount)
        {
            await rpcClient.UpdateAccountAsync(account);
            string pow = await GetPowAsync(account);

            Block receiveBlock = Block.CreateReceiveBlock(account, blockHash, amount, pow);
            await rpcClient.ProcessAsync(receiveBlock);
        }

        public async Task SendBlockAsync(Account account, string receiveAddress, Amount amount)
        {
            await rpcClient.UpdateAccountAsync(account);
            string pow = await GetPowAsync(account);

            Block sendBlock = Block.CreateSendBlock(account, receiveAddress, amount, pow);
            await rpcClient.ProcessAsync(sendBlock);
        }

        private async Task<string> GetPowAsync(Account account)
        {
            WorkGenerateResponse workGen;

            if (!account.Opened)
                workGen = await rpcClient.WorkGenerateAsync(Utils.BytesToHex(account.PublicKey));
            else
                workGen = await rpcClient.WorkGenerateAsync(account.Frontier);

            return workGen.Work;
        }
    }
}

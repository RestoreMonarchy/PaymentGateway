using Microsoft.Extensions.Options;
using Nano.Net.WebSockets;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Services
{
    public class NanoWebSocketService
    {
        private readonly WaitingNanoPaymentStore paymentStore;
        private readonly NanoOptions options;
        private readonly NanoTransactionService transactionService;

        public NanoWebSocketService(WaitingNanoPaymentStore paymentStore, IOptions<NanoOptions> options, NanoTransactionService transactionService)
        {
            this.paymentStore = paymentStore;
            this.options = options.Value;
            this.transactionService = transactionService;
        }

        private NanoWebSocketClient client;

        public async ValueTask StartAsync()
        {
            client = new NanoWebSocketClient(options.WebSocketUrl);
            
            string[] accounts = paymentStore.GetAllAddresses();
            client.Subscribe(new ConfirmationTopic(accounts: accounts));
            
            client.Confirmation += OnConfirmation;
            paymentStore.OnUpdate += OnWaitingPaymentsUpdate;

            await Task.Factory.StartNew(client.Start);
        }

        public async ValueTask StopAsync()
        {
            client.Confirmation -= OnConfirmation;
            paymentStore.OnUpdate -= OnWaitingPaymentsUpdate;

            await client.Stop();
        }

        private async void OnConfirmation(NanoWebSocketClient client, ConfirmationTopicMessage topic)
        {
            if (topic.Message.Block.Subtype != "send")
            {
                return;
            }

            NanoTransaction transaction = new()
            {
                ReceiveAddress = topic.Message.Block.LinkAsAccount,
                SendAddress = topic.Message.Block.Account,
                AmountRaw = topic.Message.Amount,
                BlockHash = topic.Message.Hash,
                IsReceiveable = true
            };

            await transactionService.ProcessTransactionAsync(transaction);            
        }

        private void OnWaitingPaymentsUpdate(WaitingNanoPayment payment)
        {
            string[] accounts = paymentStore.GetAllAddresses();
            client.UpdateSubscription(new ConfirmationTopic(accounts: accounts));
        }
    }
}

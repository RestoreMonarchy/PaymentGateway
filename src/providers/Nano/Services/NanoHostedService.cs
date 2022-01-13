using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.Net.WebSockets;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Services
{
    public class NanoHostedService : IHostedService
    {
        private readonly NanoOptions options;
        private readonly ILogger<NanoHostedService> logger;
        private readonly NanoPaymentStore paymentStore;
        private readonly NanoService nanoService;

        private readonly NanoWebSocketClient webSocketClient;

        public NanoHostedService(IOptions<NanoOptions> options, ILogger<NanoHostedService> logger, NanoPaymentStore paymentStore,
            NanoService nanoService)
        {
            this.options = options.Value;
            this.logger = logger;
            this.paymentStore = paymentStore;
            this.nanoService = nanoService;

            webSocketClient = new NanoWebSocketClient(this.options.WebSocketUrl);
            webSocketClient.Subscribe(new ConfirmationTopic(accounts: paymentStore.PendingPaymentsAddress));

            webSocketClient.Confirmation += WebSocketClient_Confirmation;
            paymentStore.OnPendingPaymentsUpdated += PaymentStore_OnPendingPaymentsUpdated;
        }

        private void PaymentStore_OnPendingPaymentsUpdated()
        {
            webSocketClient.UpdateSubscription(new ConfirmationTopic(accounts: paymentStore.PendingPaymentsAddress));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await paymentStore.ReloadPendingPaymentsAsync();
            await nanoService.CheckPendingTransactions();
            await Task.Factory.StartNew(StartWebSocketAsync);
        }

        public async Task StartWebSocketAsync()
        {
            logger.LogInformation("WebSocketClient starting...");
            await webSocketClient.Start();
            logger.LogInformation("WebSocketClient started!");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await webSocketClient.Stop();
            logger.LogInformation("WebSocketClient stopped");
        }

        private async void WebSocketClient_Confirmation(NanoWebSocketClient client, ConfirmationTopicMessage topicMessage)
        {
            if (topicMessage.Message.Block.Subtype != "send")
            {
                return;
            }

            if (!paymentStore.IsPending(topicMessage.Message.Block.LinkAsAccount))
            {
                return;
            }

            await Task.Factory.StartNew(() =>
                nanoService.CheckTransactionAsync(topicMessage.Message.Block.LinkAsAccount,
                topicMessage.Message.Hash, topicMessage.Message.Amount, topicMessage.Message.Block.Account));
        }
    }
}

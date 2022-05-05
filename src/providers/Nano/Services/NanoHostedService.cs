using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Services
{
    public class NanoHostedService : IHostedService
    {
        private readonly ILogger<NanoHostedService> logger;
        private readonly WaitingNanoPaymentStore paymentStore;
        private readonly NanoWebSocketService webSocketService;
        private readonly NanoTransactionService transactionService;

        public NanoHostedService(ILogger<NanoHostedService> logger, WaitingNanoPaymentStore paymentStore, NanoWebSocketService webSocketService, NanoTransactionService transactionService)
        {
            this.logger = logger;
            this.paymentStore = paymentStore;
            this.webSocketService = webSocketService;
            this.transactionService = transactionService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{nameof(NanoHostedService)} is starting...");
            
            await paymentStore.ReloadAsync();
            
            await transactionService.ProcessPendingBlocksAsync();
            await transactionService.EmptyBalancesAsync();

            await webSocketService.StartAsync();

            logger.LogInformation($"{nameof(NanoHostedService)} successfully started!");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{nameof(NanoHostedService)} is stopping...");

            await webSocketService.StopAsync();

            logger.LogInformation($"{nameof(NanoHostedService)} successfully stopped!");
        }
    }
}

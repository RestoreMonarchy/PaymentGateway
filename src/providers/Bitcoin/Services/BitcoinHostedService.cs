using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Services
{
    public class BitcoinHostedService : IHostedService
    {
        private readonly ILogger<BitcoinHostedService> _logger;
        private readonly WaitingBitcoinPaymentStore _paymentStore;
        private readonly BitcoinNodeService _bitcoinNodeService;

        public BitcoinHostedService(ILogger<BitcoinHostedService> logger, WaitingBitcoinPaymentStore paymentStore, BitcoinNodeService bitcoinNodeService)
        {
            _logger = logger;
            _paymentStore = paymentStore;
            _bitcoinNodeService = bitcoinNodeService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(BitcoinHostedService)} is starting...");

            await _paymentStore.ReloadAsync();
            await _bitcoinNodeService.StartAsync();

            _logger.LogInformation($"{nameof(BitcoinHostedService)} successfully started!");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(BitcoinHostedService)} is stopping...");

            await _bitcoinNodeService.StopAsync();

            _logger.LogInformation($"{nameof(BitcoinHostedService)} successfully stopped!");
        }
    }
}

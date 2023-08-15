using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Clients;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Services;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin
{
    public class BitcoinPlugin : PaymentProviderPlugin
    {
        public override string Name => "Bitcoin";

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPaymentProvider, BitcoinPaymentProvider>();

            services.AddSingleton<BitcoinEventService>();
            services.AddSingleton<BitcoinPriceService>();
            services.AddSingleton<CoinMarketCapClient>();
            services.AddSingleton<BlockCypherClient>();
            services.AddSingleton<BitcoinService>();
            services.AddSingleton<BitcoinNodeService>();
            services.AddSingleton<WaitingBitcoinPaymentStore>();

            services.Configure<BitcoinOptions>(configuration.GetSection("Bitcoin"));

            services.AddHostedService<BitcoinHostedService>();
        }

        public override void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
        }
    }
}

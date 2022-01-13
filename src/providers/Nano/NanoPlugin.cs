using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Clients;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano
{
    public class NanoPlugin : PaymentProviderPlugin
    {
        public override string Name => "Nano";

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPaymentProvider, NanoPaymentProvider>();

            services.AddTransient<CoinMarketCapClient>();
            services.AddTransient<NanoService>();
            services.AddTransient<NanoNodeClient>();
            services.AddSingleton<NanoPaymentStore>();
            services.AddHostedService<NanoHostedService>();

            services.Configure<NanoOptions>(configuration.GetSection(NanoOptions.Key));
        }

        public override void Configure(IApplicationBuilder app, IConfiguration configuration)
        {

        }
    }
}

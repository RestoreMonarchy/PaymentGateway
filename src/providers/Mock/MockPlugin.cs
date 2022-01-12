using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RestoreMonarchy.PaymentGateway.API.Abstractions;

namespace RestoreMonarchy.PaymentGateway.Providers.Mock
{
    public class MockPlugin : PaymentProviderPlugin
    {
        public override string Name => "Mock";

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPaymentProvider, MockPaymentProvider>();
        }

        public override void Configure(IApplicationBuilder app)
        {

        }
    }
}

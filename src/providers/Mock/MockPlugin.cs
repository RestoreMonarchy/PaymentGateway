using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestoreMonarchy.PaymentGateway.API.Abstractions;

namespace RestoreMonarchy.PaymentGateway.Providers.Mock
{
    public class MockPlugin : PaymentProviderPlugin
    {
        public override string Name => "Mock";

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPaymentProvider, MockPaymentProvider>();
        }
    }
}

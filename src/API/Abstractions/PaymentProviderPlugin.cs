using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RestoreMonarchy.PaymentGateway.API.Abstractions
{
    public abstract class PaymentProviderPlugin : IPaymentProviderPlugin
    {
        public abstract string Name { get; }

        public virtual void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
        }

        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }
    }
}

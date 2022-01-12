using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RestoreMonarchy.PaymentGateway.API.Abstractions
{
    public abstract class PaymentProviderPlugin : IPaymentProviderPlugin
    {
        public abstract string Name { get; }

        public virtual void Configure(IApplicationBuilder app)
        {

        }

        public virtual void ConfigureServices(IServiceCollection services)
        {

        }
    }
}

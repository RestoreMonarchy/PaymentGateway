using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.Providers.PayPal.Services;

namespace RestoreMonarchy.PaymentGateway.Providers.PayPal
{
    public class PayPalPlugin : PaymentProviderPlugin
    {
        public override string Name => "PayPal";

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<PayPalMiddleware>();
            services.AddTransient<IPaymentProvider, PayPalPaymentProvider>();
            services.AddTransient<PayPalService>();
        }

        public override void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            app.Map(
                pathMatch: "/api/payments/notify/paypal",
                configuration: b => b.UseMiddleware<PayPalMiddleware>());
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestoreMonarchy.PaymentGateway.API.Services;
using System.Text;

namespace RestoreMonarchy.PaymentGateway.Providers.PayPal.Services
{
    public class PayPalMiddleware : IMiddleware
    {
        private readonly PayPalService paypalService;
        private readonly ILoggingService loggingService;
        public PayPalMiddleware(PayPalService paypalService, ILoggingService loggingService)
        {
            this.paypalService = paypalService;
            this.loggingService = loggingService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string requestBody;
            using StreamReader reader = new(context.Request.Body, Encoding.ASCII);
            requestBody = await reader.ReadToEndAsync();

            loggingService.LogInformation<PayPalMiddleware>("Received the request. Validating it now...");

            await paypalService.ValidatePaymentAsync(requestBody);

            context.Response.StatusCode = StatusCodes.Status200OK;
        }
    }
}

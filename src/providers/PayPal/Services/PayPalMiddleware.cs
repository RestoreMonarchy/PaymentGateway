using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.PayPal.Services
{
    public class PayPalMiddleware : IMiddleware
    {
        private readonly PayPalService paypalService;
        private readonly ILogger<PayPalMiddleware> logger;
        public PayPalMiddleware(PayPalService paypalService, ILogger<PayPalMiddleware> logger)
        {
            this.paypalService = paypalService;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string requestBody;
            using StreamReader reader = new(context.Request.Body, Encoding.ASCII);
            requestBody = await reader.ReadToEndAsync();

            logger.LogInformation($"Received request at PayPal middleware");

            await paypalService.ValidatePaymentAsync(requestBody);

            context.Response.StatusCode = StatusCodes.Status200OK;
        }
    }
}

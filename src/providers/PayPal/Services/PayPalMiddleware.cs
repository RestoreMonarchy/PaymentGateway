using Microsoft.AspNetCore.Http;
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
        public PayPalMiddleware(PayPalService paypalService)
        {
            this.paypalService = paypalService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string requestBody;
            using StreamReader reader = new(context.Request.Body, Encoding.ASCII);
            requestBody = await reader.ReadToEndAsync();

            await paypalService.ValidatePaymentAsync(requestBody);

            context.Response.StatusCode = StatusCodes.Status200OK;
        }
    }
}

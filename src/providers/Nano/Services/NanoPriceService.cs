using Microsoft.Extensions.Options;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Clients;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Helpers;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Services
{
    public class NanoPriceService
    {
        private readonly CoinMarketCapClient client;
        private readonly NanoOptions options;
        private readonly IPaymentService paymentService;

        public NanoPriceService(CoinMarketCapClient client, IOptions<NanoOptions> options, IPaymentService paymentService)
        {
            this.client = client;
            this.options = options.Value;
            this.paymentService = paymentService;
        }

        public async ValueTask SetMinimumAmountAsync(PaymentInfo payment)
        {
            decimal amount = await client.ConvertToAsync(payment.Amount, payment.Currency, "XNO");
            
            NanoPaymentData paymentData = payment.Data.GetObject<NanoPaymentData>();
            
            paymentData.MinimumAmount = NumbersHelper.RoundNano(amount);
            paymentData.MinimumAmountExpireDate = DateTime.Now.AddMinutes(options.PriceRefreshMinutes);
            
            payment.Data.UpdateObject(paymentData);
        }

        public async ValueTask ValidateMinimumAmountAsync(PaymentInfo payment)
        {
            NanoPaymentData paymentData = payment.Data.GetObject<NanoPaymentData>();

            if (paymentData.MinimumAmountExpireDate >= DateTime.Now)
            {
                return;
            }

            await SetMinimumAmountAsync(payment);
            
            paymentData = payment.Data.GetObject<NanoPaymentData>();
            await paymentService.UpdatePaymentData(payment.PublicId, paymentData);
        }
    }
}

using Microsoft.Extensions.Logging;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.PayPal.Models;
using System.Collections.Specialized;
using System.Web;

namespace RestoreMonarchy.PaymentGateway.Providers.PayPal.Services
{
    public class PayPalService
    {
        private readonly IPaymentService paymentService;
        private readonly ILoggingService loggingService;

        private readonly HttpClient httpClient;

        public PayPalService(IHttpClientFactory httpClientFactory, ILoggingService loggingService, IPaymentService paymentService)
        {
            this.paymentService = paymentService;
            this.loggingService = loggingService;
            httpClient = httpClientFactory.CreateClient();            
        }

        public async Task ValidatePaymentAsync(string requestBody)
        {
            NameValueCollection dict = HttpUtility.ParseQueryString(requestBody);

            if (!Guid.TryParse(dict["custom"], out Guid publicId))
            {
                loggingService.LogInformation<PayPalService>("The 'custom' property value is not a valid GUID");
                return;
            }

            PaymentWithParameters<PayPalParameters> pwp = await paymentService.GetPaymentWithParameters<PayPalParameters>(publicId);

            if (pwp == null)
            {
                loggingService.LogInformation<PayPalService>("The payment could not be found");
                return;
            }

            StringContent content = new("cmd=_notify-validate&" + requestBody);
            HttpResponseMessage response = await httpClient.PostAsync(pwp.Parameters.GetUrl(), content);
            string verification = await response.Content.ReadAsStringAsync();

            if (!verification.Equals("VERIFIED"))
            {
                loggingService.LogInformation<PayPalService>("The request could not be verified");
                return;
            }

            string receiver = pwp.Payment.Receiver != null ? pwp.Payment.Receiver : pwp.Parameters.DefaultReceiver;

            if (dict["receiver_email"]?.Equals(receiver, StringComparison.OrdinalIgnoreCase) ?? true)
            {
                loggingService.LogInformation<PayPalService>("The 'receiver_email' property value is not equal to the payment {0} receiver", pwp.Payment.PublicId);
                return;
            }

            if (dict["mc_currency"]?.Equals(pwp.Payment.Currency, StringComparison.OrdinalIgnoreCase) ?? true)
            {
                loggingService.LogInformation<PayPalService>("The 'mc_currency' property value is not equal to the payment {0} currency", pwp.Payment.PublicId);
                return;
            }

            if (decimal.Parse(dict["mc_gross"]) < pwp.Payment.Amount)
            {
                loggingService.LogInformation<PayPalService>("The 'mc_gross' property value is not smaller than payment {0} amount", pwp.Payment.PublicId);
                return;
            }

            PayPalPaymentData paypalPayment = new()
            {
                TransactionId = dict["txn_id"],
                PayerEmail = dict["payer_email"],
                Status = dict["payment_status"],
                Gross = decimal.Parse(dict["mc_gross"]),
                Currency = dict["mc_currency"],
                CreateDate = DateTime.Now
            };

            if (dict["mc_fee"] != null)
            {
                paypalPayment.Fee = decimal.Parse(dict["mc_fee"]);
            }                

            await paymentService.UpdatePaymentData(publicId, paypalPayment);
            await paymentService.CompletePayment(publicId);
            loggingService.LogInformation<PayPalService>("The payment {0} was valid and has been completed successfully", pwp.Payment.PublicId);
        }
    }
}

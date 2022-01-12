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

        private readonly HttpClient httpClient;

        public PayPalService(IHttpClientFactory httpClientFactory, IPaymentService paymentService)
        {
            this.paymentService = paymentService;
            httpClient = httpClientFactory.CreateClient();
        }

        public async Task ValidatePaymentAsync(string requestBody)
        {
            NameValueCollection dict = HttpUtility.ParseQueryString(requestBody);

            if (!Guid.TryParse(dict["custom"], out Guid publicId))
            {
                return;
            }

            PaymentWithParameters<PayPalParameters> pwp = await paymentService.GetPaymentWithParameters<PayPalParameters>(publicId);

            if (pwp == null)
            {
                return;
            }

            StringContent content = new("cmd=_notify-validate&" + requestBody);
            HttpResponseMessage response = await httpClient.PostAsync(pwp.Parameters.GetUrl(), content);
            string verification = await response.Content.ReadAsStringAsync();

            if (!verification.Equals("VERIFIED"))
            {
                return;
            }

            string receiver;
            if (pwp.Payment.Receiver != null)
                receiver = pwp.Payment.Receiver;
            else
                receiver = pwp.Parameters.DefaultReceiver;

            if (dict["receiver_email"] != receiver)
            {
                return;
            }

            if (dict["mc_currency"] != pwp.Payment.Currency)
            {
                return;
            }

            if (decimal.Parse(dict["mc_gross"]) < pwp.Payment.Amount)
            {
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
                paypalPayment.Fee = decimal.Parse(dict["mc_fee"]);

            await paymentService.UpdatePaymentData(publicId, paypalPayment);
            await paymentService.CompletePayment(publicId);
        }
    }
}

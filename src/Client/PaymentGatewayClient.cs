using RestoreMonarchy.PaymentGateway.Client.Constants;
using RestoreMonarchy.PaymentGateway.Client.Exceptions;
using RestoreMonarchy.PaymentGateway.Client.Models;
using System.Net;
using System.Net.Http.Json;

namespace RestoreMonarchy.PaymentGateway.Client
{
    public partial class PaymentGatewayClient
    {
        private readonly PaymentGatewayClientOptions options;
        private readonly HttpClient httpClient;

        public PaymentGatewayClient(PaymentGatewayClientOptions options)
        {
            this.options = options;
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(options.BaseAddress)
            };
            httpClient.DefaultRequestHeaders.Add(PaymentGatewayConstants.APIKeyHeader, options.APIKey);
        }

        public async Task<Guid> CreatePaymentAsync(Payment payment)
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/payments", payment);
            string requestBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Guid.Parse(requestBody);
            } else
            {
                throw new CreatePaymentException(response.StatusCode, requestBody);
            }
        }
    }
}
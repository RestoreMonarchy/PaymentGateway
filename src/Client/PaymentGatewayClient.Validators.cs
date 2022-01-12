using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RestoreMonarchy.PaymentGateway.Client.Constants;

namespace RestoreMonarchy.PaymentGateway.Client
{
    public partial class PaymentGatewayClient
    {
        public bool ValdiateNotifyRequest(HttpRequest request)
        {
            if (request.Headers.TryGetValue(PaymentGatewayConstants.NotifyAPIKeyHeader, out StringValues value))
            {
                return value.Equals(options.APIKey);
            }

            return false;
        }
    }
}

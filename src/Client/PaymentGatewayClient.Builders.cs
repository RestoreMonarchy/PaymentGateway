using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Client
{
    public partial class PaymentGatewayClient
    {
        public string BuildPayUrl(Guid publicId)
        {
            return options.BaseAddress.TrimEnd('/') + $"/pay/{publicId}";
        }
    }
}

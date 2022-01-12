using Newtonsoft.Json;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;

namespace RestoreMonarchy.PaymentGateway.Models.Helpers
{
    public class PaymentWithProvider
    {
        public MPayment Payment { get; set; }
        public MStorePaymentProvider Provider { get; set; }

        public T GetParams<T>() where T : class
        {
            if (string.IsNullOrEmpty(Provider.JsonParameters))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(Provider.JsonParameters);
        }
    }
}

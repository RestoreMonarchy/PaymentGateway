using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.API.Results;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Web.Models.Exceptions;

namespace RestoreMonarchy.PaymentGateway.Web.Services
{
    public class PaymentProviders : IPaymentProviders
    {
        private readonly IEnumerable<IPaymentProvider> paymentProviders;

        public PaymentProviders(IEnumerable<IPaymentProvider> paymentProviders)
        {
            this.paymentProviders = paymentProviders ?? throw new ArgumentNullException(nameof(paymentProviders));
        }

        private IPaymentProvider GetPaymentProvider(string providerName)
        {
            IPaymentProvider provider = GetPaymentProviderSafe(providerName);

            if (provider == null)
            {
                throw new PaymentProviderNotSupportedException();
            }

            return provider;
        }

        private IPaymentProvider GetPaymentProviderSafe(string providerName)
        {
            return paymentProviders.FirstOrDefault(x => x.Name.Equals(providerName));
        }

        public Task<UserAction> StartPaymentAsync(Guid publicId, string providerName)
        {
            IPaymentProvider provider = GetPaymentProvider(providerName);
            return provider.StartPaymentAsync(publicId);
        }

        public Type GetFormComponentType(string providerName)
        {
            IPaymentProvider provider = GetPaymentProviderSafe(providerName);
            if (provider == null)
                return null;
            return provider.FormComponentType;
        }

        public Type GetInfoComponentType(string providerName)
        {
            IPaymentProvider provider = GetPaymentProviderSafe(providerName);
            if (provider == null)
                return null;
            return provider.InfoComponentType;
        }
    }
}

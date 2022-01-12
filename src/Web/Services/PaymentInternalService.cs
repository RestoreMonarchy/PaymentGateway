using RestoreMonarchy.PaymentGateway.Client.Models;
using RestoreMonarchy.PaymentGateway.Web.Extensions;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Repositories;

namespace RestoreMonarchy.PaymentGateway.Web.Services
{
    public class PaymentInternalService
    {
        public PaymentsRepository Repository { get; }
        public IConfiguration Configuration { get; }

        public PaymentInternalService(PaymentsRepository paymentsRepository, IConfiguration configuration)
        {
            Repository = paymentsRepository;
            Configuration = configuration;
        }

        public bool ValidatePayment(Payment payment, MStore store, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!Configuration.IsProviderSupported(payment.Provider))
            {
                errorMessage = "This payment provider is not supported!";
                return false;
            }

            MStorePaymentProvider provider = store.Providers.FirstOrDefault(x => x.PaymentProvider == payment.Provider);
            if (provider == null || !provider.IsEnabled)
            {
                errorMessage = "This payment provider is disabled for this store!";
                return false;
            }

            if (payment.Amount <= 0)
            {
                errorMessage = "The amount has to be greater than zero!";
                return false;
            }

            if (payment.Currency.Length != 3)
            {
                errorMessage = "The currency is not a valid currency code!";
                return false;
            }

            return true;
        }
    }
}

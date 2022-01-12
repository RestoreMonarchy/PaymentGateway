using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;

namespace RestoreMonarchy.PaymentGateway.Web.Models.View
{
    public class PaymentViewModel
    {
        public PaymentViewModel(MPayment payment, Type componentType)
        {
            Payment = payment;
            ComponentType = componentType;
        }

        public MPayment Payment { get; set; }
        public Type ComponentType { get; set; }
    }
}

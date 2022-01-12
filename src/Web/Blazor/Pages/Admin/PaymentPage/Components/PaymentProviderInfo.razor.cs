using Microsoft.AspNetCore.Components;
using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;

namespace RestoreMonarchy.PaymentGateway.Web.Blazor.Pages.Admin.PaymentPage.Components
{
    public partial class PaymentProviderInfo
    {
        [Parameter]
        public MPayment Payment { get; set; }

        [Inject]
        public IPaymentProviders PaymentProviders { get; set; }

        public Type InfoType { get; set; }

        public Dictionary<string, object> Parameters => new Dictionary<string, object>()
        {
            { "JsonDataString", Payment.JsonData }
        };

        protected override void OnParametersSet()
        {
            InfoType = PaymentProviders.GetInfoComponentType(Payment.Provider);
        }

        public DynamicComponent Component { get; set; }
    }
}

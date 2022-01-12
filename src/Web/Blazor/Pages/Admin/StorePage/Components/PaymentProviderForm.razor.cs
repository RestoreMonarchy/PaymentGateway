using Microsoft.AspNetCore.Components;
using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Repositories;

namespace RestoreMonarchy.PaymentGateway.Web.Blazor.Pages.Admin.StorePage.Components
{
    public partial class PaymentProviderForm
    {
        [Inject]
        public IPaymentProviders PaymentProviders { get; set; }

        [Parameter]
        public MStorePaymentProvider PaymentProvider { get; set; }
        [Parameter]
        public EventCallback<MStorePaymentProvider> OnSave { get; set; }
        [Parameter]
        public EventCallback<MStorePaymentProvider> OnDelete { get; set; }

        public Type FormType { get; set; }

        protected override void OnParametersSet()
        {
            FormType = PaymentProviders.GetFormComponentType(PaymentProvider.PaymentProvider);
        }

        public Dictionary<string, object> Parameters => new Dictionary<string, object>()
        {
            { "JsonDataString", PaymentProvider.JsonParameters }
        };

        public DynamicComponent Component { get; set; }

        private async Task DeleteAsync()
        {
            await OnDelete.InvokeAsync(PaymentProvider);
        }

        private bool isLoading = false;
        private async Task SubmitAsync()
        {
            isLoading = true;
            if (Component != null)
            {
                IJsonDataComponent form = (IJsonDataComponent)Component.Instance;
                PaymentProvider.JsonParameters = form.GetParametersAsJson();
            }

            await OnSave.InvokeAsync(PaymentProvider);
            isLoading = false;
        }
    }
}

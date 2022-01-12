using Microsoft.AspNetCore.Components;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;

namespace RestoreMonarchy.PaymentGateway.Providers.Mock.Components
{
    public partial class MockPayment : ComponentBase
    {
        [Inject]
        public IPaymentService PaymentService { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }

        [Parameter]
        public PaymentInfo PaymentInfo { get; set; }

        private bool isLoading = false;
        public async Task PayNow()
        {
            isLoading = true;
            await PaymentService.CompletePayment(PaymentInfo.PublicId);
            PaymentInfo.IsCompleted = true;
            NavManager.NavigateTo(PaymentInfo.Store.ReturnUrl);
        }

    }
}

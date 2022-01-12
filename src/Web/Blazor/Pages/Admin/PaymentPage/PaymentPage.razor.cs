using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Repositories;

namespace RestoreMonarchy.PaymentGateway.Web.Blazor.Pages.Admin.PaymentPage
{
    [Authorize]
    public partial class PaymentPage
    {
        [Parameter]
        public string PaymentId { get; set; }

        [Inject]
        private PaymentsRepository PaymentsRepository { get; set; }

        public MPayment Payment { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Payment = await PaymentsRepository.GetPaymentAsync(Guid.Parse(PaymentId));
        }
    }
}

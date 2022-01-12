using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Repositories;

namespace RestoreMonarchy.PaymentGateway.Web.Blazor.Pages.Admin.PaymentsPage
{
    [Authorize]
    public partial class PaymentsPage
    {
        [Inject]
        private PaymentsRepository PaymentsRepository { get; set; }

        public IEnumerable<MPayment> Payments { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Payments = await PaymentsRepository.GetPaymentsAsync();
        }
    }
}

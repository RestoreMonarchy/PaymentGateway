using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Repositories;

namespace RestoreMonarchy.PaymentGateway.Web.Blazor.Pages.Admin.StoresPage
{
    [Authorize]
    public partial class StoresPage
    {
        [Inject]
        private StoresRepository StoresRepository { get; set; }

        public IEnumerable<MStore> Stores { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Stores = await StoresRepository.GetStoresAsync();
        }
    }
}

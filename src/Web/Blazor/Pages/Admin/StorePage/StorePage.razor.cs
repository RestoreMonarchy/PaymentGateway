using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Repositories;

namespace RestoreMonarchy.PaymentGateway.Web.Blazor.Pages.Admin.StorePage
{
    [Authorize]
    public partial class StorePage
    {
        [Parameter]
        public int StoreId { get; set; }

        [Inject]
        private StoresRepository StoresRepository { get; set; }

        public MStore Store { get; set; }

        public MStore Model { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await ReloadStoreAsync();
        }

        private async Task ReloadStoreAsync()
        {
            Store = await StoresRepository.GetStoreAsync(StoreId);
            if (Store != null)
            {
                Model = Store.Clone();
            }
        }

        private async Task SaveAsync()
        {
            await StoresRepository.UpdateStoreAsync(Model);
            await ReloadStoreAsync();
        }

        private bool showAPIKey = false;
        private void ShowAPIKey()
        {
            showAPIKey = true;
        }
    }
}

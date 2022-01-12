using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Repositories;

namespace RestoreMonarchy.PaymentGateway.Web.Blazor.Pages.Admin.CreateStorePage
{
    [Authorize]
    public partial class CreateStorePage
    {
        [Inject]
        private StoresRepository StoresRepository { get; set; }
        [Inject]
        private NavigationManager NavManager { get; set; }

        public MStore Model { get; set; } = new MStore();

        private async Task SubmitAsync()
        {
            int storeId = await StoresRepository.AddStoreAsync(Model);
            NavManager.NavigateTo($"/Admin/Stores/{storeId}");
        }
    }
}
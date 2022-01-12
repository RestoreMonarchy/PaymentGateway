using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestoreMonarchy.PaymentGateway.Web.Blazor.Services;
using RestoreMonarchy.PaymentGateway.Web.Extensions;

namespace RestoreMonarchy.PaymentGateway.Web.Blazor.Shared.Layouts
{
    public partial class AdminLayout    
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; }
        [Inject]
        public BlazorUser User { get; set; }

        public async Task ToggleSidebar()
        {
            await JsRuntime.ToggleBodyClassAsync("sb-sidenav-toggled");
        }
    }
}

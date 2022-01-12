using Microsoft.JSInterop;

namespace RestoreMonarchy.PaymentGateway.Web.Extensions
{
    public static class IJSRuntimeExtensions
    {
        public static async Task ToggleBodyClassAsync(this IJSRuntime jsRuntime, string className)
        {
            await jsRuntime.InvokeVoidAsync("ToggleBodyClass", className);
        }
    }
}

using Microsoft.AspNetCore.Components;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Web.Extensions;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Repositories;

namespace RestoreMonarchy.PaymentGateway.Web.Blazor.Pages.Admin.StorePage.Components
{
    public partial class PaymentProvidersTab
    {
        [Parameter]
        public MStore Store { get; set; }

        [Inject]
        public StoresRepository StoresRepository { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }


        public string[] SupportedProviders => Configuration.GetSupportedProviders();
        public IEnumerable<string> AvailableProviders
            => SupportedProviders.Where(x => !Store.Providers.Exists(y => y.PaymentProvider == x));

        public async Task AddProviderAsync(string name)
        {
            MStorePaymentProvider provider = new MStorePaymentProvider()
            {
                StoreId = Store.Id,
                PaymentProvider = name,
                JsonParameters = null,
                IsEnabled = false,
                CreateDate = DateTime.Now
            };
            Store.Providers.Add(provider);
            await SaveAsync(provider);
        }

        public async Task SaveAsync(MStorePaymentProvider provider)
        {
            await StoresRepository.UpdateStorePaymentProviderAsync(provider);
        }

        private async Task DeleteProviderAsync(MStorePaymentProvider provider)
        {
            Store.Providers.Remove(provider);
            await StoresRepository.DeleteStorePaymentProviderAsync(provider);
        }
    }
}

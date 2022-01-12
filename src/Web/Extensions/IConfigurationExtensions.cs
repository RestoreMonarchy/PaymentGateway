namespace RestoreMonarchy.PaymentGateway.Web.Extensions
{
    public static class IConfigurationExtensions
    {
        public static string[] GetSupportedProviders(this IConfiguration configuration)
        {
            return configuration.GetSection("SupportedProviders").Get<string[]>() ?? new string[0];
        }
        public static bool IsProviderSupported(this IConfiguration configuration, string providerName)
        {
            string[] enabledProviders = GetSupportedProviders(configuration);
            if (enabledProviders.Any(x => x.Equals(providerName, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
            return false;
        }
    }
}

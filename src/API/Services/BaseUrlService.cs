using Microsoft.Extensions.Configuration;

namespace RestoreMonarchy.PaymentGateway.API.Services
{
    public class BaseUrlService : IBaseUrl
    {
        public BaseUrlService(IConfiguration configuration)
        {
            BaseUrl = configuration["BaseUrl"].TrimEnd('/');
        }

        public string BaseUrl { get; }

        public string Get(string relativeUrl)
        {
            relativeUrl = relativeUrl.TrimStart('/');
            return BaseUrl + "/" + relativeUrl;
        }
    }
}

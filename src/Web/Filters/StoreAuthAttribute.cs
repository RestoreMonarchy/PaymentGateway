using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Repositories;

namespace RestoreMonarchy.PaymentGateway.Web.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class StoreAuthAttribute : Attribute, IAsyncActionFilter
    {
        public const string APIKeyHeaderName = "x-api-key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(APIKeyHeaderName, out StringValues potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!Guid.TryParse(potentialApiKey, out Guid apiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            StoresRepository storesRepository = context.HttpContext.RequestServices.GetRequiredService<StoresRepository>();
            MStore store = await storesRepository.GetStoreAsync(apiKey);

            if (store == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.HttpContext.Items["Store"] = store;
            await next();
        }
    }
}

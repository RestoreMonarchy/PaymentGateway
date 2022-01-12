namespace RestoreMonarchy.PaymentGateway.Web.Blazor.Services
{
    public class BlazorUser
    {
        private readonly IHttpContextAccessor accessor;

        public BlazorUser(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public bool IsAuthenticated => accessor.HttpContext.User.Identity.IsAuthenticated;
        public string Name => accessor.HttpContext.User.Identity?.Name ?? string.Empty;
    }
}

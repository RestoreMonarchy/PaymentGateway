using RestoreMonarchy.PaymentGateway.API.Results;

namespace RestoreMonarchy.PaymentGateway.API.Abstractions
{
    public abstract class PaymentProvider : IPaymentProvider
    {
        public abstract string Name { get; }
        public abstract Type FormComponentType { get; }
        public abstract Type InfoComponentType { get; }
        public abstract Task<UserAction> StartPaymentAsync(Guid publicId);

        protected UserAction Redirect(string url) => new RedirectAction(url);
        protected UserAction Content(string content) => new ContentAction(content);
        protected UserAction RazorComponent(Type type) => new RazorComponentAction(type);
    }
}

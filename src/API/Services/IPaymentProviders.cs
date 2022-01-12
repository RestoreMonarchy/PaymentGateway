using RestoreMonarchy.PaymentGateway.API.Results;

namespace RestoreMonarchy.PaymentGateway.API.Services
{
    public interface IPaymentProviders
    {
        Type GetFormComponentType(string provider);
        Type GetInfoComponentType(string provider);
        Task<UserAction> StartPaymentAsync(Guid publicId, string provider);
    }
}

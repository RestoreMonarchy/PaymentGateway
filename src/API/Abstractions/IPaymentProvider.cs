using RestoreMonarchy.PaymentGateway.API.Results;

namespace RestoreMonarchy.PaymentGateway.API.Abstractions
{
    public interface IPaymentProvider
    {
        string Name { get; }
        Type FormComponentType { get; }
        Type InfoComponentType { get; }

        Task<UserAction> StartPaymentAsync(Guid publicId);
    }
}

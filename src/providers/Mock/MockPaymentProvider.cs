using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.API.Results;
using RestoreMonarchy.PaymentGateway.Providers.Mock.Components;

namespace RestoreMonarchy.PaymentGateway.Providers.Mock
{
    public class MockPaymentProvider : PaymentProvider
    {
        public override string Name => "Mock";
        public override Type FormComponentType => null;
        public override Type InfoComponentType => null;

        public override Task<UserAction> StartPaymentAsync(Guid publicId)
        {
            return Task.FromResult(RazorComponent(typeof(MockPayment)));
        }
    }
}
using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.API.Models;

namespace RestoreMonarchy.PaymentGateway.API.Services
{
    public interface IPaymentService
    {
        Task<PaymentInfo> GetPaymentInfo(Guid publicId);
        Task<PaymentWithParameters<TParameters>> GetPaymentWithParameters<TParameters>(Guid publicId);
        Task CompletePayment(Guid publicId);
        Task UpdatePaymentData(Guid publicId, object data);
    }
}

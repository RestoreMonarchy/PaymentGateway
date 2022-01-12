using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.API.Services
{
    public class TestPaymentService : IPaymentService
    {
        public Task CompletePayment(Guid publicId)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentInfo> GetPaymentInfo(Guid publicId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentWithParameters<T>> GetPaymentWithParameters<T>(Guid publicId)
        {
            return new PaymentWithParameters<T>(null, null);
        }

        public Task UpdatePaymentData(Guid publicId, object data)
        {
            throw new NotImplementedException();
        }
    }
}

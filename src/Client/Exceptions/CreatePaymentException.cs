using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Client.Exceptions
{
    public class CreatePaymentException : Exception
    {
        public CreatePaymentException(HttpStatusCode statusCode, string message) 
            : base($"[{(int)statusCode} - {statusCode}] Message: {message}") 
        {

        }
    }
}

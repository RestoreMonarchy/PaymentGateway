using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.API.Services
{
    public interface IBaseUrl
    {
        string BaseUrl { get; }
        string Get(string relativeUrl);
    }
}

using RestoreMonarchy.PaymentGateway.Providers.Nano.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Services
{
    public class NanoNodeService
    {
        private readonly NanoNodeClient client;

        public NanoNodeService(NanoNodeClient client)
        {
            this.client = client;
        }

    }
}

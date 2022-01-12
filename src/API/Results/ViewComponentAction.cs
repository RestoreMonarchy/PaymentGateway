using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.API.Results
{
    public class RazorComponentAction : UserAction
    {
        public RazorComponentAction(Type componentType)
        {
            this.ComponentType = componentType;
        }

        public Type ComponentType { get; set; }
    }
}

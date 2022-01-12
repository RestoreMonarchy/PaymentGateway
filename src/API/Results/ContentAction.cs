using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.API.Results
{
    public class ContentAction : UserAction
    {
        public ContentAction(string htmlContent)
        {
            Content = htmlContent;
        }

        public string Content { get; }
    }
}

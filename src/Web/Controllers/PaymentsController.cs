using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Results;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Client.Models;
using RestoreMonarchy.PaymentGateway.Models.Helpers;
using RestoreMonarchy.PaymentGateway.Web.Filters;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Models.Exceptions;
using RestoreMonarchy.PaymentGateway.Web.Models.View;
using RestoreMonarchy.PaymentGateway.Web.Services;

namespace RestoreMonarchy.PaymentGateway.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : Controller
    {
        private readonly PaymentInternalService paymentInternalService;
        private readonly IPaymentService paymentService;
        private readonly IPaymentProviders paymentProviders;
        private readonly ApplicationPartManager _applicationPartManager;

        public PaymentsController(PaymentInternalService paymentInternalService, IPaymentService paymentService, 
            IPaymentProviders paymentProviders, ApplicationPartManager applicationPartManager)
        {
            this.paymentInternalService = paymentInternalService;
            this.paymentService = paymentService;
            this.paymentProviders = paymentProviders;
            _applicationPartManager = applicationPartManager;
        }

        [HttpPost]
        [StoreAuth]
        public async Task<IActionResult> PostAsync([FromBody] Payment payment)
        {
            MStore store = (MStore)HttpContext.Items["Store"];

            if (!paymentInternalService.ValidatePayment(payment, store, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            Guid publicId = await paymentInternalService.Repository.AddPaymentAsync(MPayment.FromPayment(payment, store.Id));
            return Ok(publicId.ToString());
        }

        [HttpGet("~/pay/{publicId}")]
        public async Task<IActionResult> Pay([FromRoute] Guid publicId)
        {
            PaymentWithProvider pwp = await paymentInternalService.Repository.GetPaymentWithProviderAsync(publicId);

            if (pwp.Provider == null || !pwp.Provider.IsEnabled)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            UserAction action;
            try
            {
                action = await paymentProviders.StartPaymentAsync(publicId, pwp.Payment.Provider);                
            } catch (PaymentProviderNotSupportedException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (action is RedirectAction)
            {
                RedirectAction act = (RedirectAction)action;
                return Redirect(act.Url);
            } else if (action is ContentAction)
            {
                ContentAction act = (ContentAction)action;
                return Content(act.Content);
            } else if (action is RazorComponentAction)
            {
                RazorComponentAction act = (RazorComponentAction)action;
                pwp.Payment = await paymentInternalService.Repository.GetPaymentAsync(publicId);
                return View("/Views/Payments/Payment.cshtml", new PaymentViewModel(pwp.Payment, act.ComponentType));
            } else
            {
                throw new NotImplementedException();
            }
        }
    }
}
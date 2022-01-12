using Newtonsoft.Json;
using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Models.Helpers;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Repositories;

namespace RestoreMonarchy.PaymentGateway.Web.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentsRepository paymentsRepository;
        private readonly NotifyService notifyService;

        public PaymentService(PaymentsRepository paymentsRepository, NotifyService notifyService)
        {
            this.paymentsRepository = paymentsRepository;
            this.notifyService = notifyService;
        }

        public async Task CompletePayment(Guid publicId)
        {
            await paymentsRepository.CompletePaymentAsync(publicId);
            MPayment payment = await paymentsRepository.GetPaymentWithStoreAsync(publicId);
            await notifyService.BeginNotifyAsync(payment);
        }

        public async Task<PaymentInfo> GetPaymentInfo(Guid publicId)
        {
            MPayment payment = await paymentsRepository.GetPaymentAsync(publicId);

            if (payment == null)
            {
                return null;
            }

            return CreatePaymentInfo(payment);
        }

        public async Task<PaymentWithParameters<TParameters>> GetPaymentWithParameters<TParameters>(Guid publicId)
        {
            PaymentWithProvider pwp = await paymentsRepository.GetPaymentWithProviderAsync(publicId);

            if (pwp == null)
            {
                return null;
            }

            return new PaymentWithParameters<TParameters>(CreatePaymentInfo(pwp.Payment), new JsonData(pwp.Provider.JsonParameters));
        }

        public async Task UpdatePaymentData(Guid publicId, object data)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            await paymentsRepository.UpdatePaymentDataAsync(publicId, jsonData);
        }

        private PaymentInfo CreatePaymentInfo(MPayment payment)
        {
            PaymentInfo info = new()
            {
                PublicId = payment.PublicId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Receiver = payment.Receiver,
                IsCompleted = payment.IsCompleted,
                Data = new JsonData(payment.JsonData),
                Store = new StoreInfo()
                {
                    Name = payment.Store.Name,
                    LogoUrl = payment.Store.LogoUrl,
                    BackgroundUrl = payment.Store.BackgroundUrl,
                    ReturnUrl = payment.Store.DefaultReturnUrl,
                    CancelUrl = payment.Store.DefaultCancelUrl
                },
                Items = new List<PaymentItemInfo>()
            };

            foreach (MPaymentItem item in payment.Items)
            {
                info.Items.Add(new PaymentItemInfo()
                {
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                });
            }

            return info;
        }
    }
}

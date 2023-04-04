using Quartz;
using RestoreMonarchy.PaymentGateway.Client.Constants;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Repositories;
using RestoreMonarchy.PaymentGateway.Web.Services.Jobs;
using System.Net;

namespace RestoreMonarchy.PaymentGateway.Web.Services
{
    public class NotifyService
    {
        public PaymentsRepository PaymentsRepository { get; }
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<NotifyService> logger;
        private readonly ISchedulerFactory schedulerFactory;

        public NotifyService(PaymentsRepository paymentsRepository, IHttpClientFactory httpClientFactory,
            ILogger<NotifyService> logger, ISchedulerFactory schedulerFactory)
        {
            PaymentsRepository = paymentsRepository;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.schedulerFactory = schedulerFactory;
        }

        public async Task BeginNotifyAsync(MPayment payment)
        {
            if (await SendNotifyAsync(payment))
            {
                return;
            }

            IJobDetail job = JobBuilder.Create<PaymentNotifyJob>()
                .UsingJobData("PaymentPublicId", payment.PublicId)
                .StoreDurably()
                .Build();
            ISimpleTrigger trigger = PaymentNotifyJob.BuildTrigger(job, 1, IntervalUnit.Minute);
            IScheduler scheduler = await schedulerFactory.GetScheduler("Job Scheduler");
            await scheduler.ScheduleJob(job, trigger);
        }

        public async Task<bool> SendNotifyAsync(MPayment payment)
        {
            HttpClient httpClient = httpClientFactory.CreateClient("Default");
            httpClient.DefaultRequestHeaders.Add(PaymentGatewayConstants.NotifyAPIKeyHeader, payment.Store.APIKey.ToString());
            StringContent content = new(payment.PublicId.ToString());

            try
            {
                HttpResponseMessage msg = await httpClient.PostAsync(payment.Store.DefaultNotifyUrl, content);
                if (msg.StatusCode == HttpStatusCode.OK)
                {
                    payment.NotifiedCount++;
                    await PaymentsRepository.SetNotifiedAsync(payment);
                    logger.LogInformation("Successfully notified {0} about payment {1}", payment.Store.Name, payment.PublicId);
                    return true;
                }
            }
            catch (HttpRequestException e)
            {
                logger.LogWarning("Unable to notify the store {0} with the message: {1}", payment.Store.DefaultNotifyUrl, e.Message);
            }

            payment.NotifiedCount++;
            await PaymentsRepository.IncrementNotifiedCountAsync(payment);

            return false;
        }
    }
}

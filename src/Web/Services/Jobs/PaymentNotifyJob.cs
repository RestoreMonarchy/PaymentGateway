using Quartz;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;

namespace RestoreMonarchy.PaymentGateway.Web.Services.Jobs
{
    public class PaymentNotifyJob : IJob
    {
        private readonly NotifyService notifyService;

        public PaymentNotifyJob(NotifyService notifyService)
        {
            this.notifyService = notifyService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Guid publicId = context.JobDetail.JobDataMap.GetGuidValue("PaymentPublicId");

            MPayment payment = await notifyService.PaymentsRepository.GetPaymentWithStoreAsync(publicId);

            if (payment.IsNotified)
                return;

            if (await notifyService.SendNotifyAsync(payment))
            {
                return;
            }

            ISimpleTrigger trigger = null;
            if (payment.NotifiedCount <= 3)
            {
                trigger = BuildTrigger(context.JobDetail, 3, IntervalUnit.Minute);
            } else if (payment.NotifiedCount <= 5)
            {
                trigger = BuildTrigger(context.JobDetail, 10, IntervalUnit.Minute);
            } else if (payment.NotifiedCount <= 10)
            {
                trigger = BuildTrigger(context.JobDetail, 30, IntervalUnit.Minute);
            } else if (payment.NotifiedCount <= 30)
            {
                trigger = BuildTrigger(context.JobDetail, 3, IntervalUnit.Hour);   
            }

            if (trigger != null)
                await context.Scheduler.RescheduleJob(context.Trigger.Key, trigger);
        }

        public static ISimpleTrigger BuildTrigger(IJobDetail job, int interval, IntervalUnit unit)
        {
            return (ISimpleTrigger)TriggerBuilder.Create()
                .StartAt(DateBuilder.FutureDate(interval, unit))
                .ForJob(job)
                .Build();
        }
    }
}

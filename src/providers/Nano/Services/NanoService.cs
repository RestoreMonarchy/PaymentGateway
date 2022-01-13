using Nano.Net;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Clients;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Helpers;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Services
{
    public class NanoService
    {
        private readonly NanoNodeClient nodeClient;
        private readonly NanoPaymentStore paymentStore;
        private readonly IPaymentService paymentService;

        public NanoService(NanoNodeClient nodeClient, NanoPaymentStore paymentStore, IPaymentService paymentService)
        {
            this.nodeClient = nodeClient;
            this.paymentStore = paymentStore;
            this.paymentService = paymentService;
        }

        public async Task CheckPendingTransactions()
        {
            if (!paymentStore.PendingPayments.Any())
                return;

            Dictionary<string, Dictionary<string, ReceivableBlock>> dict = await nodeClient.GetAccountsPendingAsync(paymentStore.PendingPaymentsAddress);
            foreach (string account in dict.Keys)
            {
                Dictionary<string, ReceivableBlock> transaction = dict[account];
                if (transaction == null)
                    continue;

                foreach (string blockHash in transaction.Keys)
                {
                    ReceivableBlock block = transaction[blockHash];
                    await CheckTransactionAsync(account, blockHash, block.Amount, block.Source);
                }
            }
        }

        public async Task CheckTransactionAsync(string receiverAddress, string blockHash, string amountRaw, string senderAddress)
        {
            PaymentInfo payment = paymentStore.GetPendingPaymentByReceiver(receiverAddress);
            if (payment == null)
            {
                return;
            }

            NanoPaymentData paymentData = payment.Data.GetObject<NanoPaymentData>();

            Account account = Account.FromPrivateKey(paymentData.ReceivePrivateKey);
            Amount amount = Amount.FromRaw(amountRaw);

            await nodeClient.ReceiveBlockAsync(account, blockHash, amount);

            await paymentStore.TryUpdateMinimumAmountAsync(payment);

            if (amount.Nano >= paymentData.MinimumAmount)
            {
                decimal nanoAmount = NumbersHelper.RoundNano(amount.Nano);
                await paymentStore.ReceivePaymentAsync(payment, senderAddress, nanoAmount);

                // Send nano to seller if the amount is valid
                await nodeClient.SendBlockAsync(account, payment.Receiver, amount);

                await paymentService.CompletePayment(payment.PublicId);
            }
            else
            {
                // Refund nano to sender if amount is invalid
                await nodeClient.SendBlockAsync(account, senderAddress, amount);
            }
        }
    }
}

using Nano.Net;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Clients;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Services
{
    public class NanoTransactionService
    {
        private readonly WaitingNanoPaymentStore paymentStore;
        private readonly IPaymentService paymentService;
        private readonly NanoPriceService priceService;
        private readonly NanoNodeClient nodeClient;
        private readonly NanoEventService eventService;

        public NanoTransactionService(WaitingNanoPaymentStore paymentStore, IPaymentService paymentService, NanoPriceService priceService, NanoNodeClient nodeClient, NanoEventService eventService)
        {
            this.paymentStore = paymentStore;
            this.paymentService = paymentService;
            this.priceService = priceService;
            this.nodeClient = nodeClient;
            this.eventService = eventService;
        }

        public async ValueTask ProcessPendingBlocksAsync()
        {
            string[] accounts = paymentStore.GetAllAddresses();
            if (!accounts.Any())
                return;


            Dictionary<string, Dictionary<string, ReceivableBlock>> dict = await nodeClient.GetAccountsPendingAsync(accounts);
            foreach (string address in dict.Keys)
            {
                Dictionary<string, ReceivableBlock> pair = dict[address];
                if (pair == null || !pair.Any()) 
                {
                    continue;
                }

                if (!paymentStore.TryGetByReceiveAddress(address, out WaitingNanoPayment waitingPayment))
                {
                    continue;
                }

                PaymentInfo payment = await paymentService.GetPaymentInfo(waitingPayment.PublicId);
                NanoPaymentData paymentData = payment.Data.GetObject<NanoPaymentData>();
                Account account = new(paymentData.ReceivePrivateKey);

                foreach (string blockHash in pair.Keys)
                {
                    ReceivableBlock block = pair[blockHash];
                    NanoTransaction transaction = new()
                    {
                        ReceiveAddress = address,
                        SendAddress = block.Source,
                        AmountRaw = block.Amount,
                        BlockHash = block.Hash,
                        IsReceiveable = true
                    };

                    await ProcessTransactionAsync(payment, transaction);
                }
            }
        }

        public async ValueTask EmptyBalancesAsync()
        {
            string[] accounts = paymentStore.GetAllAddresses();
            if (!accounts.Any())
                return;

            Dictionary<string, BalancePendingPair> dict = await nodeClient.GetAccountsBalancesAsync(accounts);
            foreach (string address in dict.Keys)
            {
                BalancePendingPair pair = dict[address];
                if (pair == null)
                {
                    continue;
                }                    

                if (pair.Balance == "0")
                {
                    continue;
                }

                if (!paymentStore.TryGetByReceiveAddress(address, out WaitingNanoPayment waitingPayment))
                {
                    continue;
                }

                PaymentInfo payment = await paymentService.GetPaymentInfo(waitingPayment.PublicId);
                NanoPaymentData paymentData = payment.Data.GetObject<NanoPaymentData>();

                //HistoryBlock[] blocks = await nodeClient.GetAccountHistoryAsync(address);
                //foreach (HistoryBlock block in blocks)
                //{
                //    if (block.Hash == paymentData.PaymentBlock)
                //    {
                //        continue;
                //    }
                //}

                Account account = new(paymentData.ReceivePrivateKey);
                await nodeClient.SendBalanceAsync(account, payment.Receiver);
            }
        }

        public async ValueTask ProcessTransactionAsync(NanoTransaction transaction)
        {
            if (!paymentStore.TryGetByReceiveAddress(transaction.ReceiveAddress, out WaitingNanoPayment waitingPayment))
            {
                return;
            }

            PaymentInfo payment = await paymentService.GetPaymentInfo(waitingPayment.PublicId);
            await ProcessTransactionAsync(payment, transaction);
        }

        public async ValueTask ProcessTransactionAsync(PaymentInfo payment, NanoTransaction transaction)
        {            
            NanoPaymentData paymentData = payment.Data.GetObject<NanoPaymentData>();
            
            Account receiveAccount = new(paymentData.ReceivePrivateKey);

            if (transaction.IsReceiveable)
                await nodeClient.ReceiveBlockAsync(receiveAccount, transaction.BlockHash, transaction.Amount);

            if (payment.IsCompleted)
            {
                await Refund();
                return;
            }

            await priceService.ValidateMinimumAmountAsync(payment);

            if (transaction.RoundedAmount >= paymentData.MinimumAmount)
            {
                await nodeClient.SendBlockAsync(receiveAccount, payment.Receiver, transaction.Amount);
                
                paymentData.PaidAmount = transaction.RoundedAmount;
                paymentData.PayerAddress = transaction.SendAddress;
                paymentData.ReceiveDate = DateTime.Now;
                paymentData.PaymentBlock = transaction.BlockHash;
                await paymentService.UpdatePaymentData(payment.PublicId, paymentData);
                
                await paymentService.CompletePayment(payment.PublicId);
                eventService.TriggerOnPaymentReceived(payment.PublicId);
            } else
            {
                await Refund();
            }

            async Task Refund()
            {
                await nodeClient.SendBlockAsync(receiveAccount, transaction.SendAddress, transaction.Amount);
            }
        }
    }
}

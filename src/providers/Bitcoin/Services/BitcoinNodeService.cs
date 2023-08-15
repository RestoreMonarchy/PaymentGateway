using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Protocol;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Clients;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Services
{
    public class BitcoinNodeService
    {
        private Node _connectedNode;
        private FeeRate _feeRate;
        private readonly WaitingBitcoinPaymentStore _paymentStore;
        private readonly IPaymentService _paymentService;
        private readonly BlockCypherClient _blockCypherClient;
        private readonly BitcoinOptions _options;
        private BitcoinEventService _bitcoinEventService;
        private readonly ILogger<BitcoinNodeService> _logger;

        public BitcoinNodeService(
            WaitingBitcoinPaymentStore paymentStore,
            IPaymentService paymentService,
            IOptions<BitcoinOptions> options,
            BlockCypherClient blockCypherClient,
            BitcoinEventService bitcoinEventService,
            ILogger<BitcoinNodeService> logger)
        {
            _paymentStore = paymentStore;
            _paymentService = paymentService;
            _options = options.Value;
            _blockCypherClient = blockCypherClient;
            _bitcoinEventService = bitcoinEventService;
            _logger = logger;
        }

        public async Task StartAsync()
        {
            _logger.LogDebug("Connecting to node..");
            _connectedNode = await Node.ConnectAsync(Network.Main, _options.NodeAddress);
            _connectedNode.VersionHandshake();
            _connectedNode.MessageReceived += OnMessage;
        }

        private async Task TransactAllBalanceAsync(byte[] senderPrivateKey, string receiveAddress)
        {
            var key = new Key(senderPrivateKey).GetBitcoinSecret(Network.Main);
            var sendBtcAddress = key.GetAddress(ScriptPubKeyType.Segwit);
            var receiveBtcAddress = BitcoinAddress.Create(receiveAddress, Network.Main);

            string sendBtcAddresString = sendBtcAddress.ToString();

            _logger.LogInformation("Sending all balance from {sendBtcAddresString} to {receiveBtcAddress}", sendBtcAddresString, receiveAddress);

            var unSpentCoins = await _blockCypherClient.GetUnSpentCoinsAsync(sendBtcAddresString);

            var total = unSpentCoins.Sum(c => c.Amount.ToDecimal(MoneyUnit.BTC));

            _logger.LogDebug("Deteceted {Sum} in wallet {sendBtcAddresString}", total, sendBtcAddresString);

            var builder = Network.Main.CreateTransactionBuilder();
            builder.AddCoins(unSpentCoins);
            builder.AddKeys(key);
            builder.SendEstimatedFees(_feeRate);
            builder.SendAll(receiveBtcAddress);
            builder.SetChange(sendBtcAddress);

            var res = builder.BuildTransaction(true);

            var result = builder.Verify(res, out var errors);
            if (!result)
            {
                _logger.LogError("Failed to verify transaction");
                foreach (var error in errors)
                {
                    _logger.LogInformation(error.ToString());
                }
                return;
            }

            _logger.LogDebug("Sending inv payload..");
            await _connectedNode.SendMessageAsync(new InvPayload(InventoryType.MSG_TX, res.GetHash()));
            _logger.LogDebug("Sending transaction payload..");
            await _connectedNode.SendMessageAsync(res.CreatePayload());
            _logger.LogInformation("Transaction complete from {sendBtcAddresString} to {receiveBtcAddress} with hash {transactionHash}", sendBtcAddresString, receiveAddress, res.GetHash());
        }

        private void OnMessage(Node node, IncomingMessage message)
        {
            message.Message.IfPayloadIs<InvPayload>(async invPayload =>
            {
                try
                {
                    foreach (var inventory in invPayload.Inventory)
                    {
                        if (inventory.Type != InventoryType.MSG_BLOCK)
                            return;

                        _logger.LogDebug("Detected block {Hash}", inventory.Hash);

                        await _connectedNode.SendMessageAsync(new GetDataPayload(new[] { new InventoryVector(InventoryType.MSG_BLOCK, inventory.Hash) }));
                        var block = _connectedNode.ReceiveMessage<BlockPayload>(TimeSpan.FromMinutes(30)).Object;
                        _logger.LogDebug("Block has {Count} transactions", block.Transactions.Count);

                        foreach (var transaction in block.Transactions.Skip(1))
                        {
                            var btcTransaction = new BitcoinTransaction
                            {
                                SendAddress = transaction.Inputs.First().WitScript.ToScript().WitHash.ScriptPubKey.GetDestinationAddress(Network.Main).ToString(),
                                Outputs = transaction.Outputs.Where(o => o.Value.ToDecimal(MoneyUnit.BTC) != 0m).Select(o => (o.ScriptPubKey.GetDestinationAddress(Network.Main).ToString(), o.Value.ToDecimal(MoneyUnit.BTC))).ToList(),
                                TransactionHash = transaction.GetHash().ToString()
                            };
                            await ProcessTransactionAsync(btcTransaction);
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "There was an error while working with the block transactions");
                }
            });
            message.Message.IfPayloadIs<FeeFilterPayload>(async payload =>
            {
                var averageFees = await _blockCypherClient.GetAverageFeesAsync();

                if (averageFees.SatoshiPerByte < payload.FeeRate.SatoshiPerByte)
                {
                    _feeRate = payload.FeeRate;
                }
                else
                {
                    _feeRate = averageFees;
                }

                _logger.LogDebug("Fee Rate set to {FeeRate}", _feeRate);
            });
        }

        private async Task ProcessTransactionAsync(BitcoinTransaction transaction)
        {
            WaitingBitcoinPayment? waitingPayment = null;

            foreach (var output in transaction.Outputs)
            {
                if (_paymentStore.TryGetByReceiveAddress(output.ReceiveAddress, out waitingPayment))
                    break;
            }

            if (waitingPayment is null)
                return;

            PaymentInfo payment = await _paymentService.GetPaymentInfo(waitingPayment.PublicId);
            await ProcessTransactionAsync(payment, transaction);
        }

        private async Task ProcessTransactionAsync(PaymentInfo payment, BitcoinTransaction transaction)
        {
            BitcoinPaymentData paymentData = payment.Data.GetObject<BitcoinPaymentData>();
            var sentAmount = transaction.Outputs.Where(o => o.ReceiveAddress == paymentData.ReceiveAddress).Sum(o => o.Amount);
            _logger.LogDebug("Processing transaction from {SendAddress} to {ReceiveAddress} of {Amount} for payment {PublicId}", transaction.SendAddress, paymentData.ReceiveAddress, sentAmount, payment.PublicId);

            if (sentAmount >= paymentData.MinimumAmount)
            {
                _logger.LogDebug("Sending money to the seller..");

                await TransactAllBalanceAsync(paymentData.ReceivePrivateKey, payment.Receiver);
                paymentData.PayerAddress = transaction.SendAddress;
                paymentData.PaidAmount = sentAmount;
                paymentData.PaymentHash = transaction.TransactionHash;
                paymentData.ReceiveDate = DateTime.Now;

                _logger.LogDebug("Updating payment data and completing payment..");

                await _paymentService.UpdatePaymentData(payment.PublicId, paymentData);
                await _paymentService.CompletePayment(payment.PublicId);
                _paymentStore.RemoveByPublicId(payment.PublicId);
                _bitcoinEventService.TriggerOnPaymentReceived(payment.PublicId);
            }
            else
            {
                _logger.LogDebug("Payment amount is invalid. Refunding...");
                await TransactAllBalanceAsync(paymentData.ReceivePrivateKey, transaction.SendAddress);
            }
        }

        public Task StopAsync()
        {
            _connectedNode?.DisconnectAsync();
            return Task.CompletedTask;
        }
    }
}

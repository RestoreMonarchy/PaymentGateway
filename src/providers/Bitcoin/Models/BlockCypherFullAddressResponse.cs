using System.Text.Json.Serialization;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models
{
    public class BlockCypherFullAddressResponse
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("total_teceived")]
        public long TotalReceived { get; set; }
        [JsonPropertyName("total_sent")]
        public long TotalSent { get; set; }
        [JsonPropertyName("balance")]
        public long Balance { get; set; }
        [JsonPropertyName("unconfirmed_balance")]
        public long UnConfirmedBalance { get; set; }
        [JsonPropertyName("final_balance")]
        public long FinalBalance { get; set; }
        [JsonPropertyName("n_tx")]
        public long NTx { get; set; }
        [JsonPropertyName("unconfirmed_n_tx")]
        public long UnCconfirmedNTx { get; set; }
        [JsonPropertyName("final_n_tx")]
        public long FinalNTx { get; set; }
        [JsonPropertyName("txs")]
        public List<Tx> Txs { get; set; }

        public class Tx
        {
            [JsonPropertyName("block_hash")]
            public string BlockHash { get; set; }
            [JsonPropertyName("block_height")]
            public long BlockHeight { get; set; }
            [JsonPropertyName("block_index")]
            public long BlockIndex { get; set; }
            [JsonPropertyName("hash")]
            public string Hash { get; set; }
            [JsonPropertyName("addresses")]
            public List<string> Addresses { get; set; }
            [JsonPropertyName("total")]
            public long Total { get; set; }
            [JsonPropertyName("fees")]
            public long Fees { get; set; }
            [JsonPropertyName("size")]
            public long Size { get; set; }
            [JsonPropertyName("vsize")]
            public long VSize { get; set; }
            [JsonPropertyName("preference")]
            public string Preference { get; set; }
            [JsonPropertyName("relayed_by")]
            public string RelayedBy { get; set; }
            [JsonPropertyName("confirmed")]
            public DateTime Confirmed { get; set; }
            [JsonPropertyName("received")]
            public DateTime Received { get; set; }
            [JsonPropertyName("ver")]
            public long Ver { get; set; }
            [JsonPropertyName("double_spend")]
            public bool DoubleSpend { get; set; }
            [JsonPropertyName("vin_sz")]
            public long VinSz { get; set; }
            [JsonPropertyName("vout_sz")]
            public long VoutSz { get; set; }
            [JsonPropertyName("opt_in_rbf")]
            public bool OptInRbf { get; set; }
            [JsonPropertyName("confirmations")]
            public long Confirmations { get; set; }
            [JsonPropertyName("confidence")]
            public long Confidence { get; set; }
            [JsonPropertyName("inputs")]
            public List<Input> Inputs { get; set; }
            [JsonPropertyName("outputs")]
            public List<Output> Outputs { get; set; }
            [JsonPropertyName("lock_time")]
            public long LockTime { get; set; }
        }

        public class Input
        {
            [JsonPropertyName("prev_hash")]
            public string PrevHash { get; set; }
            [JsonPropertyName("output_index")]
            public long OutputIndex { get; set; }
            [JsonPropertyName("output_value")]
            public long OutputValue { get; set; }
            [JsonPropertyName("Sequence")]
            public long Sequence { get; set; }
            [JsonPropertyName("addresses")]
            public List<string> Addresses { get; set; }
            [JsonPropertyName("script_type")]
            public string ScriptType { get; set; }
            [JsonPropertyName("age")]
            public long Age { get; set; }
            [JsonPropertyName("witness")]
            public List<string> Witness { get; set; }
        }

        public class Output
        {
            [JsonPropertyName("value")]
            public int Value { get; set; }
            [JsonPropertyName("script")]
            public string Script { get; set; }
            [JsonPropertyName("spent_by")]
            public string SpentBy { get; set; }
            [JsonPropertyName("addresses")]
            public List<string> Addresses { get; set; }
            [JsonPropertyName("script_type")]
            public string ScriptType { get; set; }
        }

    }
}

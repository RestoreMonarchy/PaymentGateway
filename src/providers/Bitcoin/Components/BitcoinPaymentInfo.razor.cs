using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Components
{
    public partial class BitcoinPaymentInfo
    {
        private bool isShow = false;
        private void HandleShow()
        {
            isShow = true;
        }

        private string PrivateKeyHex => BytesToHex(Data.ReceivePrivateKey);

        public static string BytesToHex(byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);

            return hex.ToString();
        }
    }
}

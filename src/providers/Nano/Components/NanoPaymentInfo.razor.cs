using System.Text;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Components
{
    public partial class NanoPaymentInfo
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

            return hex.ToString().ToUpper();
        }
    }
}

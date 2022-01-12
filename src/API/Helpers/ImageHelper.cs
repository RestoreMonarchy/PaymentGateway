using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace RestoreMonarchy.PaymentGateway.API.Helpers
{
    public class ImageHelper
    {
        public static Tuple<byte[], string> GenerateQRCode(string content)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap graphic = qrCode.GetGraphic(20);
            using MemoryStream ms = new();
            graphic.Save(ms, ImageFormat.Jpeg);
            return new Tuple<byte[], string>(ms.ToArray(), "image/jpeg");
        }
    }
}

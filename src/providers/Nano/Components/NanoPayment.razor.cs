using Microsoft.AspNetCore.Components;
using QRCoder;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Models;
using RestoreMonarchy.PaymentGateway.Providers.Nano.Services;
using System.Drawing;
using System.Drawing.Imaging;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Components
{
    public partial class NanoPayment
    {
        [Parameter]
        public PaymentInfo PaymentInfo { get; set; }

        [Inject]
        public NanoEventService EventService { get; set; }

        public NanoPaymentData Data { get; set; }

        protected override void OnParametersSet()
        {
            Data = PaymentInfo.Data.GetObject<NanoPaymentData>();
            IsReceived = Data.PaymentBlock != null;
            EventService.OnPaymentReceived += OnPaymentReceived;
            UpdateQRCode();            
        }

        public bool IsReceived { get; set; }
        private async void OnPaymentReceived(Guid publicId)
        {
            if (publicId != PaymentInfo.PublicId)
                return;

            await InvokeAsync(() =>
            {
                IsReceived = true;
                StateHasChanged();
            });
        }

        public string QRCodeBase64 { get; set; }

        public void UpdateQRCode()
        {
            byte[] codeData = GenerateQRCode(Data.ReceiveAddress);
            QRCodeBase64 = Convert.ToBase64String(codeData);
        }

        public static byte[] GenerateQRCode(string content)
        {
            QRCodeGenerator generator = new();
            QRCodeData data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new(data);
            Bitmap graphic = code.GetGraphic(20);
            using MemoryStream ms = new();
            graphic.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }
    }
}

using Microsoft.AspNetCore.Components;
using QRCoder;
using RestoreMonarchy.PaymentGateway.API.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Services;
using RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Components
{
    public partial class BitcoinPayment
    {
        [Parameter]
        public PaymentInfo PaymentInfo { get; set; }

        [Inject]
        public BitcoinEventService EventService { get; set; }

        public BitcoinPaymentData Data { get; set; }

        protected override void OnParametersSet()
        {
            Data = PaymentInfo.Data.GetObject<BitcoinPaymentData>();
            IsReceived = Data.PaymentHash != null;
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

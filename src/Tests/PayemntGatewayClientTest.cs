using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestoreMonarchy.PaymentGateway.Client;
using RestoreMonarchy.PaymentGateway.Client.Constants;
using RestoreMonarchy.PaymentGateway.Client.Models;
using System;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.Tests
{
    [TestClass]
    public class PayemntGatewayClientTest
    {
        const string LocalAddress = "https://localhost:7255";
        const string LocalAPIKey = "b20ba663-b240-4b9b-9be5-31d1a346032f";
        const string ServerBaseAddress = "https://pay.restoremonarchy.com";
        const string ServerAPIKey = "d0ee1508-1819-4e40-9ffc-ea76a222d1de";

        const string BaseAddress = LocalAddress;
        const string APIKey = LocalAPIKey;

        const string ReceiverMock = "receiver@email.com";
        const string ReceiverNano = "nano_33i6ngzch919195mxuhi98cyxf7mngwa8i381bfd4tyqhesiyhew9jk5kqtq";
        const string ReceiverPayPal = "tomasz.wrona-facilitator@hotmail.com";

        [TestMethod]
        public void Payment_SyncAmount()
        {
            Payment payment = Payment.Create(PaymentProviders.Mock, "123", ReceiverMock, "USD", 14);

            payment.AddItem("Cheeseburger", 5, 2);
            payment.AddItem("Large Fries", 2, 2);

            Assert.IsTrue(payment.ValidateAmount());
        }

        [TestMethod]
        public async Task PaymentGatewayClient_CreatePayment_Mock_10()
        {
            PaymentGatewayClient client = new(new PaymentGatewayClientOptions() 
            { 
                BaseAddress = BaseAddress,
                APIKey = APIKey                
            });

            Payment payment = Payment.Create(PaymentProviders.Mock, "123", ReceiverMock, "USD", 49);

            payment.AddItem("Large Fries", 5, 2);
            payment.AddItem("Cheeseburger", 2, 2);
            payment.AddItem("Big Mac", 10, 3.5m);

            for (int i = 0; i < 10; i++)
            {
                Guid publicId = await client.CreatePaymentAsync(payment);
                Assert.AreNotSame(Guid.Empty, publicId);
            }            
        }

        [TestMethod]
        public async Task PaymentGatewayClient_CreatePayment_Nano()
        {
            PaymentGatewayClient client = new(new PaymentGatewayClientOptions()
            {
                BaseAddress = BaseAddress,
                APIKey = APIKey
            });

            Payment payment = Payment.Create(PaymentProviders.Nano, "123", ReceiverNano, "USD", 0.01m);

            payment.AddItem("Large Fries", 1, 0.01m);

            Guid publicId = await client.CreatePaymentAsync(payment);
            Assert.AreNotSame(Guid.Empty, publicId);
        }

        [TestMethod]
        public async Task PaymentGatewayClient_CreatePayment_PayPal()
        {
            PaymentGatewayClient client = new(new PaymentGatewayClientOptions()
            {
                BaseAddress = BaseAddress,
                APIKey = APIKey
            });

            Payment payment = Payment.Create(PaymentProviders.PayPal, "123", ReceiverPayPal, "USD", 49);

            payment.AddItem("Large Fries", 5, 2);
            payment.AddItem("Cheeseburger", 2, 2);
            payment.AddItem("Big Mac", 10, 3.5m);

            Guid publicId = await client.CreatePaymentAsync(payment);
            Assert.AreNotSame(Guid.Empty, publicId);
        }

        [TestMethod]
        public async Task PaymentGatewayClient_CreatePayment_Przelewy24()
        {
            PaymentGatewayClient client = new(new PaymentGatewayClientOptions()
            {
                BaseAddress = BaseAddress,
                APIKey = APIKey
            });

            Payment payment = Payment.Create(PaymentProviders.Przelewy24, "123", null, "PLN", 174);

            payment.AddItem("Large Fries", 5, 6);
            payment.AddItem("Cheeseburger", 2, 4.5m);
            payment.AddItem("Big Mac", 10, 13.5m);

            Guid publicId = await client.CreatePaymentAsync(payment);
            Assert.AreNotSame(Guid.Empty, publicId);
        }
    }
}
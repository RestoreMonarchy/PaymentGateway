//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Nano.Net.Numbers;
//using System.Numerics;

//namespace RestoreMonarchy.PaymentGateway.Tests
//{
//    [TestClass]
//    public class PaymentGatewayNanoTest
//    {
//        [TestMethod]
//        public void NumbersHelper_RoundNano()
//        {
//            BigDecimal amount = new BigDecimal(0.00182);
//            decimal nano = NumbersHelper.RoundNano(amount);
//            Assert.AreEqual(0.0018m, nano);
//        }

//        [TestMethod]
//        public void NumbersHelper_RoundNanoString()
//        {
//            BigDecimal amount = new BigDecimal(0.00182);
//            decimal nano = NumbersHelper.RoundNano(decimal.Parse(amount.ToString()));
//            Assert.AreEqual(0.0018m, nano);
//        }

//        [TestMethod]
//        public void NumbersHelper_RoundNanoFromBigInteger()
//        {
//            BigInteger bigInteger = BigInteger.Parse("1820000000000000000000000000");

//            var raw = (BigDecimal)bigInteger;

//            var toNano = BigDecimal.Parse("0.000000000000000000000000000001");

//            BigDecimal amount = new BigDecimal(false, 30);

//            amount = raw * toNano;

//            decimal nano = NumbersHelper.RoundNano(amount);
//            Assert.AreEqual(0.0018m, nano);
//        }
//    }
//}

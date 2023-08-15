
namespace RestoreMonarchy.PaymentGateway.Providers.Bitcoin.Helpers
{
    public static class NumbersHelper
    {
        public static decimal RoundBitcoin(decimal value)
        {
            return Math.Round(value, 8);
        }
    }
}

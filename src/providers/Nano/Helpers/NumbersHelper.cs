using Nano.Net.Numbers;

namespace RestoreMonarchy.PaymentGateway.Providers.Nano.Helpers
{
    public class NumbersHelper
    {
        public static decimal RoundNano(BigDecimal amount)
        {
            return RoundNano(decimal.Parse(amount.ToString()));
        }

        public static decimal RoundNano(decimal amount)
        {
            return Math.Round(amount, 4);
        }
    }
}

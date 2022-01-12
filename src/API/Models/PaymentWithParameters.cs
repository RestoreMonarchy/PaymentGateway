namespace RestoreMonarchy.PaymentGateway.API.Models
{
    public class PaymentWithParameters<T>
    {
        public PaymentWithParameters(PaymentInfo payment, JsonData jsonData)
        {
            Payment = payment;
            Parameters = jsonData.GetObject<T>();
        }

        public PaymentInfo Payment { get; set; }
        public T Parameters { get; set; }
    }
}

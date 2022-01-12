namespace RestoreMonarchy.PaymentGateway.API.Results
{
    public class RedirectAction : UserAction
    {
        public RedirectAction(string url)
        {
            Url = url;
        }

        public string Url { get; }
    }
}

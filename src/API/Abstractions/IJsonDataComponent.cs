namespace RestoreMonarchy.PaymentGateway.API.Abstractions
{
    public interface IJsonDataComponent
    {
        bool IsLoaded { get; }

        string GetParametersAsJson();
        void LoadParametersFromJson(string json);
    }
}

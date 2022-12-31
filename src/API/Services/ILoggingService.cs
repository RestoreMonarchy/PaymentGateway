namespace RestoreMonarchy.PaymentGateway.API.Services
{
    public interface ILoggingService
    {
        void LogError(Exception exception, string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogInformation<T>(string message, params object[] args);
        void LogWarning(string message, params object[] args);
    }
}

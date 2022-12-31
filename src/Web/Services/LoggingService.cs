using RestoreMonarchy.PaymentGateway.API.Services;

namespace RestoreMonarchy.PaymentGateway.Web.Services
{
    public class LoggingService : ILoggingService
    {
        public void LogInformation(string message, params object[] args)
        {
            message = string.Format(message, args);

            Serilog.Log.Information(message);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            message = string.Format(message, args);

            Serilog.Log.Error(exception, message);
        }

        public void LogWarning(string message, params object[] args)
        {
            message = string.Format(message, args);

            Serilog.Log.Warning(message);
        }

        public void LogInformation<T>(string message, params object[] args)
        {
            message = string.Format(message, args);
            message = $"[{nameof(T)}] {message}";

            Serilog.Log.Information(message);
        }
    }
}

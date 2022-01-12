using Newtonsoft.Json;

namespace RestoreMonarchy.PaymentGateway.API.Models
{
    public class JsonData
    {
        public string Json { get; }
        public JsonData(string json)
        {
            Json = json;
        }

        public T GetObject<T>()
        {
            if (Json == null)
                return default;

            return JsonConvert.DeserializeObject<T>(Json);
        }
    }
}

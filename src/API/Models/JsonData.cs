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

        private object Object { get; set; }

        public T GetObject<T>()
        {
            if (Object == null)
            {
                if (Json == null)
                    return default;

                Object = JsonConvert.DeserializeObject<T>(Json);
            }

            return (T)Object;
        }

        public void UpdateObject(object obj)
        {
            Object = obj;
        }
    }
}

using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RestoreMonarchy.PaymentGateway.API.Abstractions;

namespace RestoreMonarchy.PaymentGateway.API.Components
{
    public abstract class JsonDataComponentBase<T> : ComponentBase, IJsonDataComponent
    {
        [Parameter]
        public string JsonDataString { get; set; }

        public T Data { get; private set; }

        public bool IsLoaded => Data != null;

        protected override void OnParametersSet()
        {
            LoadParametersFromJson(JsonDataString);
        }

        public string GetParametersAsJson()
        {
            return JsonConvert.SerializeObject(Data);
        }

        public void LoadParametersFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                Data = Activator.CreateInstance<T>();
            } else
            {
                Data = JsonConvert.DeserializeObject<T>(json);
            }
            
            StateHasChanged();
        }
    }
}

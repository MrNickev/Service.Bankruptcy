using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.Common.Models;

internal class ConfigurationBase<TModel> where TModel : class
{
    [JsonExtensionData]
    private IDictionary<string, JToken>? _additionalData;

    [JsonExtensionData]
    protected IDictionary<string, JToken>? AdditionalData
    {
        get => _additionalData ??= new Dictionary<string, JToken>();
        set => _additionalData = value;
    }

    /// <summary>
    /// Модель конфигурации.
    /// </summary>
    [JsonIgnore]
    internal TModel? Model
    {
        get
        {
            var data = AdditionalData?[typeof(TModel).Name];

            return data is null ? default : JsonConvert.DeserializeObject<TModel>(data.ToString());
        }
    }
}
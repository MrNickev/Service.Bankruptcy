using System.Text.Json.Serialization;

namespace Application.Fedresurs.Models;

/// <summary>
/// Модель ответа на поисковый запрос на сайте Федресурса
/// </summary>
/// <typeparam name="T">Тип получаемых данных</typeparam>
public class PageData<T>
{
    /// <summary>
    /// Количество найденных объектов
    /// </summary>
    public int Total { get; set; }
    
    /// <summary>
    /// Список найденных объектов 
    /// </summary>
    [JsonPropertyName("pageData")]
    public List<T> Data { get; set; }
}
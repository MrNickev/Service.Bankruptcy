namespace Application.Common.Exceptions;

/// <summary>
/// Ошибка отправки запроса
/// </summary>
public class RequestException : Exception
{
    public int StatusCode { get; set; }
    
    /// <summary>
    /// Конструктор ошибки запроса
    /// </summary>
    /// <param name="message">Сообщение об ошибки</param>
    public RequestException(string message) : base(message)
    {
    }

    /// <summary>
    /// Конструктор ошибки запроса
    /// </summary>
    /// <param name="statusCode">Код ошибки</param>
    /// <param name="message">Сообщение ошибки</param>
    public RequestException(int statusCode, string message) : this(
        $"Request failed with status code {statusCode}: {message}")
    {
        StatusCode = statusCode;
    }
}
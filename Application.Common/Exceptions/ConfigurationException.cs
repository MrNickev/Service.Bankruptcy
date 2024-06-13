namespace Application.Common.Exceptions;

/// <summary>
/// Ошибка конфигурации приложения
/// </summary>
public class ConfigurationException : Exception
{
    /// <summary>
    /// Коструктор ошибки конфигурации
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    public ConfigurationException(string message) : this("ERROR: Configuration;", message)
    {
    }

    /// <summary>
    /// Конструктор ошибки в конфигурации.
    /// </summary>
    /// <param name="preMessage">Приставка к сообщению об ошибки (А-ля "Provider: Siab;")</param>
    /// <param name="message">Сообщение об ошибки</param>
    public ConfigurationException(string preMessage, string message) : base($"{preMessage} {message}")
    {
    }
}
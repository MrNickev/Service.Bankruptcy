namespace Application.Fedresurs.Abstractions;

/// <summary>
/// Сервис для авторизации на сервисе проверки
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Получить токен для авторизации
    /// </summary>
    /// <returns></returns>
    public Task<string> GetToken();
}
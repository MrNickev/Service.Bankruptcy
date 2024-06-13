namespace Application.Fedresurs.Abstractions;

public interface IAuthService
{
    /// <summary>
    /// Получить токен для авторизации
    /// </summary>
    /// <returns></returns>
    public Task<string> GetToken();
}
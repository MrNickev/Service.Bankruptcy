namespace Application.Fedresurs.Models;

/// <summary>
/// Данные банкрота-компании
/// </summary>
public class Company
{
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// ОГРН
    /// </summary>
    public string Ogrn { get; set; }
    
    /// <summary>
    /// ИНН
    /// </summary>
    public string Inn { get; set; }
    
    /// <summary>
    /// Адрес
    /// </summary>
    public string Address { get; set; }
}
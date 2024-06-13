namespace Application.Fedresurs.Models;

/// <summary>
/// Данные банкрота-физ. лица
/// </summary>
public class Person
{
    /// <summary>
    /// Имя
    /// </summary>
    public string FirstName { get; set; }
    
    /// <summary>
    /// Фамилия
    /// </summary>
    public string LastName { get; set; }
    
    /// <summary>
    /// Отчество
    /// </summary>
    public string MiddleName { get; set; }
    
    /// <summary>
    /// ИНН
    /// </summary>
    public string Inn { get; set; }
    
    /// <summary>
    /// СНИЛС
    /// </summary>
    public string Snils { get; set; }
    
    /// <summary>
    /// ОГРНИП
    /// </summary>
    public string Ogrnip { get; set; }
    
    /// <summary>
    /// Место рождения
    /// </summary>
    public string Birthplace { get; set; }
    
    /// <summary>
    /// Дата рождения
    /// </summary>
    public DateTime Birthdate { get; set; }
    
    /// <summary>
    /// Адрес
    /// </summary>
    public string Address { get; set; }
    
    /// <summary>
    /// Список ранее имевшихся ФИО
    /// </summary>
    public List<string> NameHistory { get; set; }
}
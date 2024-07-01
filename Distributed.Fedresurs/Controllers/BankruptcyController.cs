using Application.Common.Astractions;
using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Models;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Fedresurs.Controllers;

/// <summary>
/// Контроллер проверки на банкротство
/// </summary>
/// <param name="bankruptcyCheckServices">Сервис проверки на банкротство</param>
[ApiController]
[Route("bankruptcy")]
public class BankruptcyController(IEnumerable<IBankruptcyCheckService> bankruptcyCheckServices) : ControllerBase
{
    
    /// <summary>
    /// Проверка на банкротство на все сервисах првоерки на банкротство
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Результаты проверок в формате массива объктов </returns>
    /// <response code="200">Успешное выполнение</response>
    /// <response code="400">Запрос не выполнен по причине некорректных данных</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpGet]
    [Route("")]
    public IActionResult Check([FromQuery] BankruptcyCheckRequest request)
    {
        try
        {
            var tasks = bankruptcyCheckServices.Select(service => service.Check(request)).ToArray();
            var results = Task.WhenAll(tasks);
            return Ok(results.Result.ToList());
        }
        catch (Exception e)
        {
            if (e is RequestException)
            {
                return StatusCode(((RequestException)e).StatusCode, "Ошибка запроса на сторонний сервис:" + e.Message);
            }

            return StatusCode(500, e.Message);
        }
        
    }
    
    
}
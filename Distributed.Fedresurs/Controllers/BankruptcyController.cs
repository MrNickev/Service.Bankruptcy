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
    /// <returns>Результаты проверок в формате массива объектов </returns>
    /// <response code="200">Успешное выполнение</response>
    /// <response code="400">Запрос не выполнен по причине некорректных данных</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpPost]
    [Route("/check")]
    public IActionResult Check(BankruptcyCheckRequest request)
    {
        try
        {
            var tasks = bankruptcyCheckServices.Select(service => service.Check(request)).ToArray();
            var results = Task.WhenAll(tasks);
            var groupedResults = results.Result
                .GroupBy(r => r.Request)
                .Select(g => new ClientBankruptcyCheckResult(
                    g.Key,
                    g.SelectMany(r => r.Results).ToList()
                ))
                .ToList();
            
            return Ok(groupedResults);
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
    
    /// <summary>
    /// Проверка на банкротство множества клиентов на все сервисах проверки на банкротство
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Результаты проверок в формате массива объектов </returns>
    /// <response code="200">Успешное выполнение</response>
    /// <response code="400">Запрос не выполнен по причине некорректных данных</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpPost]
    [Route("/multipleCheck")]
    public async Task<IActionResult> Check(List<BankruptcyCheckRequest> requests)
    {
        try
        {
            var tasks = new List<Task<ClientBankruptcyCheckResult>>();
            foreach (var request in requests)
            {
                tasks.AddRange(bankruptcyCheckServices.Select(service => service.Check(request)).ToArray());
            }
            var results = await Task.WhenAll(tasks);

            var groupedResults = results
                .GroupBy(r => r.Request)
                .Select(g => new ClientBankruptcyCheckResult(
                    g.Key,
                    g.SelectMany(r => r.Results).ToList()
                ))
                .ToList();
            return Ok(groupedResults);
        }
        catch (Exception e)
        {
            if (e is RequestException)
            {
                return StatusCode(((RequestException)e).StatusCode, "Ошибка запроса на сторонний сервис:" + e.Message);
            }
        
            return StatusCode(500, e.Message + e.InnerException?.Message);
        }
        
    }
    
    
}
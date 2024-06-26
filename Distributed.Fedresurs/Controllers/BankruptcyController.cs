using Application.Common.Astractions;
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
    /// <returns></returns>
    [HttpGet]
    [Route("")]
    public IActionResult Check([FromQuery] BankruptcyCheckRequest request)
    {
        var tasks = bankruptcyCheckServices.Select(service => service.Check(request)).ToArray();
        var results = Task.WhenAll(tasks);

        return Ok(results.Result.ToList());
    }
    
    
}
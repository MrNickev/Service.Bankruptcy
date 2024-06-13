using Application.Common.Astractions;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Models;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Fedresurs.Controllers;

/// <summary>
/// Контроллер проверки на банкротство на Федресурсе
/// </summary>
/// <param name="bankruptcyCheckService">Сервис проверки на банкротство</param>
[ApiController]
[Route("fedresurs/bankrutcy")]
public class BankruptcyController(IBankruptcyCheckService bankruptcyCheckService) : ControllerBase
{
    [HttpGet]
    [Route("")]
    public async Task<BankruptCheckResult> Check([FromQuery] BankruptcyCheckRequest request)
    {
        
        return await bankruptcyCheckService.Check(request);
    }
    
    
}
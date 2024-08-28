using System.Threading.RateLimiting;

namespace Application.Common.Handlers;

/// <summary>
/// Хэндлер-ограничитель для HttpClient. Ограничивает отправку запросов, согласно установленному <see cref="RateLimiter"/>
/// </summary>
/// <param name="limiter">Ограничитель</param>
public class ClientSideLimitedHandler(
    RateLimiter limiter)
    : DelegatingHandler(new HttpClientHandler()), IAsyncDisposable
{
    /// <summary>
    /// Ждет, пока не появится возможность отправить запрос, затем отправляет его
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        await WaitUntilReplenished(cancellationToken);
        
        return await base.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Метод резервирует ресурсы для отправки запроса или ждет пока они не освободятся
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async Task WaitUntilReplenished(CancellationToken cancellationToken)
    {
        var lease = await limiter.AcquireAsync(cancellationToken: cancellationToken);
        if (!lease.IsAcquired)
        {
            //ждем пока не появится место в очереди
            await limiter.AcquireAsync(0);
            await WaitUntilReplenished(cancellationToken);
        }
    }
    
    
    async ValueTask IAsyncDisposable.DisposeAsync()
    { 
        await limiter.DisposeAsync().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            limiter.Dispose();
        }
    }
}
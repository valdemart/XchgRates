using System.Net;

namespace XchgRates.Server.Infrastructure.Extensions;

internal static class HttpClientExtensions
{
    extension(HttpClient client)
    {
        /// <param name="requestExecutor">Performs the request</param>
        /// <param name="retryCheck">Verifies if retry should occur. Args: last status code, attempt number. Return value: false - don't retry (and return current HttpResponseMessage), true - retry</param>
        /// <param name="retryDelay">Returns time (in milliseconds) of the delay to wait before next retry. Args: last status code (or null on exception), attempt number. Return value: delay time</param>
        /// <param name="exceptionHandler">Handles exceptions. Args: exception, attempt number. Return value: false - rethrow (stops retries and causes the exception to bubble up), true - retry (silence the exception and retry)</param>
        public async Task<HttpResponseMessage> RetryOnError(Func<HttpClient, Task<HttpResponseMessage>> requestExecutor, Func<HttpStatusCode, int, bool> retryCheck, Func<HttpStatusCode?, int, int> retryDelay, Func<Exception, int, bool>? exceptionHandler = null)
        {
            for (int attempt = 1; true; attempt++)
            {
                try
                {
                    var response = await requestExecutor(client);

                    if (response.IsSuccessStatusCode || !retryCheck(response.StatusCode, attempt))
                        return response;

                    await Task.Delay(retryDelay(response.StatusCode, attempt));
                }
                catch (Exception ex)
                {
                    if (exceptionHandler == null || !exceptionHandler(ex, attempt))
                        throw;

                    await Task.Delay(retryDelay(null, attempt));
                }
            }
        }
    }
}

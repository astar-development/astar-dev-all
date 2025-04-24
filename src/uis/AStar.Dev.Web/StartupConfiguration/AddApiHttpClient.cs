using System.Net;
using System.Security.Cryptography;
using AStar.Dev.Web.Fakes;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace AStar.Dev.Web.StartupConfiguration;

public static class AddApiHttpClient
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TApiClient"></typeparam>
    /// <typeparam name="TApiConfiguration"></typeparam>
    public static void AddApiClient<TApiClient, TApiConfiguration>(this IServiceCollection services)
        where TApiClient : class
        where TApiConfiguration : class, IApiConfiguration
        => _ = services.AddHttpClient<TApiClient>()
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                    .AddPolicyHandler(GetRetryPolicy())
                    .AddMicrosoftIdentityUserAuthenticationHandler(
                                                                   nameof(TApiClient),
                                                                   options => options.Scopes = "api://2ca26585-5929-4aae-86a7-a00c3fc2d061/user.read")
                    .ConfigureHttpClient((serviceProvider, client) =>
                                         {
                                             client.BaseAddress = new Uri("");// serviceProvider.GetRequiredService<IOptions<TApiConfiguration>>().Value.BaseUrl;

                                             client.DefaultRequestHeaders.Accept.Add(new("application/json"));
                                         })
                    .AddResilienceHandler($"{nameof(TApiClient)}Handler",
                                          b => b.AddFallback(new()
                                                             {
                                                                 FallbackAction = _ => Outcome.FromResultAsValueTask(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)),
                                                             })
                                                .AddConcurrencyLimiter(100));

    // .AddRetry(new HttpRetryStrategyOptions())
    // .AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions())
    // .AddTimeout(new HttpTimeoutStrategyOptions()));

    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        int jitter = RandomNumberGenerator.GetInt32(500, 1500);

        return HttpPolicyExtensions
              .HandleTransientHttpError()

               //.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
              .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) + jitter));
    }
}

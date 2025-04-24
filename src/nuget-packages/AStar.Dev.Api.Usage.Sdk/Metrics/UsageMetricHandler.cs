using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace AStar.Dev.Api.Usage.Sdk.Metrics;

/// <summary>
/// </summary>
/// <param name="next"></param>
/// <param name="send"></param>
public sealed class UsageMetricHandler(RequestDelegate next, Send send)
{
    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context)
    {
        HttpRequest request     = context.Request;
        string      httpMethod  = request.Method;
        string      apiEndpoint = request.Path;
        string?     apiName     = request.Host.Host;

        apiName = UpdateApiNameIfRequired(apiName);

        long startTimestamp = Stopwatch.GetTimestamp();

        await next(context);

        long     endTimestamp = Stopwatch.GetTimestamp();
        TimeSpan diff         = Stopwatch.GetElapsedTime(startTimestamp, endTimestamp);

        await send.SendUsageEventAsync(new(apiName, apiEndpoint, httpMethod, diff.TotalMilliseconds, context.Response.StatusCode), CancellationToken.None);
    }

    private static string UpdateApiNameIfRequired(string apiName)
    {
        if (apiName == "host.docker.internal")
        {
            apiName = "astar.dev.images.api";
        }

        return apiName;
    }
}

﻿using System.Text.Json;
using AStar.Dev.Api.HealthChecks;
using AStar.Dev.Images.Api.Client.SDK.Models;
using AStar.Dev.Technical.Debt.Reporting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;

namespace AStar.Dev.Images.Api.Client.SDK.ImagesApi;

/// <summary>
///     The <see href="Images.ApiClient"></see> class
/// </summary>
/// <param name="httpClient"></param>
/// <param name="tokenAcquisitionService"></param>
/// <param name="logger"></param>
[Refactor(5, 10, "This class needs to be refactored / rewritten")]
public sealed class ImagesApiClient(HttpClient httpClient, ITokenAcquisition tokenAcquisitionService, ILogger<ImagesApiClient> logger) : IApiClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    /// <inheritdoc />
    public async Task<HealthStatusResponse> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Checking the {ApiName} Health Status.", Constants.ApiName);

            HttpResponseMessage response = await httpClient.GetAsync("/health/live", cancellationToken);

            return response.IsSuccessStatusCode
                       ? await ReturnLoggedSuccess(response)
                       : ReturnLoggedFailure(response);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(500, ex, "Error: {ErrorMessage}", ex.Message);

            return new() { Status = $"Could not get a response from the {Constants.ApiName}.", };
        }
    }

    private async Task<HealthStatusResponse> ReturnLoggedSuccess(HttpResponseMessage response)
    {
        logger.LogInformation("The {ApiName} Health check completed successfully.", Constants.ApiName);

        return (await JsonSerializer.DeserializeAsync<HealthStatusResponse>(await response.Content.ReadAsStreamAsync(),
                                                                            JsonSerializerOptions))!;
    }

    private HealthStatusResponse ReturnLoggedFailure(HttpResponseMessage response)
    {
        logger.LogInformation("The {ApiName} Health failed - {FailureReason}.", Constants.ApiName,
                              response.ReasonPhrase);

        return new() { Status = $"Health Check failed - {response.ReasonPhrase}.", };
    }

    /// <summary>
    /// </summary>
    /// <param name="imagePath"></param>
    /// <param name="maximumSizeInPixels"></param>
    /// <param name="thumbnail"></param>
    /// <returns></returns>
    [Refactor(1, 1, "Refactor the param: thumbnail as well as the passed value")]
    public async Task<Stream> GetImageAsync(string imagePath, int maximumSizeInPixels, bool thumbnail)
    {
        var    requestUri = $"image?imagePath={Uri.EscapeDataString(imagePath)}&maximumSizeInPixels={maximumSizeInPixels}&thumbnail={thumbnail}&version=1.0";
        string token      = await tokenAcquisitionService.GetAccessTokenForUserAsync(["api://54861ab2-fdb0-4e18-a073-c90e7bf9f0c5/ToDoList.Write",]);

        // logger.LogDebug("Token: {Token}", token);
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);

        httpClient.Timeout = TimeSpan.FromMinutes(1); // was erroring on 30 seconds
        HttpResponseMessage response = await httpClient.GetAsync(requestUri);

        return response.IsSuccessStatusCode
                   ? await response.Content.ReadAsStreamAsync()
                   : CreateNotFoundMemoryStream(imagePath);
    }

    private Stream CreateNotFoundMemoryStream(string fileName)
    {
        logger.LogInformation("Could not find: {FileName}", fileName);

        return NotFound.Image;
    }
}

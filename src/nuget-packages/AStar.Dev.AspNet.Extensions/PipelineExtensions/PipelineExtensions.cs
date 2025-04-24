﻿using Asp.Versioning.ApiExplorer;
using AStar.Dev.Api.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AStar.Dev.AspNet.Extensions.PipelineExtensions;

/// <summary>
///     The <see cref="ServiceCollectionExtensions" /> class contains the method(s) available to configure the pipeline in
///     a consistent manner.
/// </summary>
public static class PipelineExtensions
{
    /// <summary>
    ///     The <see cref="UseApiServices" /> will configure the pipeline to include Swagger, Authentication, Authorisation
    ///     and basic live/ready health check endpoints
    /// </summary>
    /// <param name="app">
    ///     The instance of the <see cref="WebApplication" /> to configure.
    /// </param>
    /// <param name="enableSwaggerDarkMode">Controls whether to enable Swagger-UI Dark Mode. The default is: true</param>
    /// <returns>
    ///     The instance of the <see cref="WebApplication" /> to facilitate chaining.
    /// </returns>
    public static WebApplication UseApiServices(this WebApplication app, bool enableSwaggerDarkMode = true)
    {
        app.UseExceptionHandler();
        app.ConfigureHealthCheckEndpoints().UseSwagger();

        if (enableSwaggerDarkMode)
        {
            app.UseStaticFiles();
        }

        app.UseSwaggerUI(SetupAction(app, enableSwaggerDarkMode))
           .UseAuthentication()
           .UseAuthorization();

        return app;
    }

    private static Action<SwaggerUIOptions> SetupAction(WebApplication webApplication,
                                                        bool           enableSwaggerDarkMode = true)
        => options =>
        {
            IReadOnlyList<ApiVersionDescription> descriptions = webApplication.DescribeApiVersions();

            foreach (ApiVersionDescription? description in descriptions)
            {
                var    url  = $"/swagger/{description.GroupName}/swagger.json";
                string name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);

                if (enableSwaggerDarkMode)
                {
                    options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
                }
            }
        };
}

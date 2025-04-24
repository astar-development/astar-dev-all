using System.IO.Abstractions;
using AStar.Dev.Api.Usage.Sdk;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.Web;
using AStar.Dev.Web.Services;
using AStar.Dev.Web.StartupConfiguration;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using RabbitMQ.Client;
using Serilog;

string applicationName = typeof(IAssemblyMarker).Assembly.GetName().Name!;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.AddSerilogLogging(Configuration.ExternalSettingsFile);

    Log.Information("Starting {AppName}", applicationName);

    IServiceCollection services = builder.Services;

    services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);

    string[]? toDoScopes = builder.Configuration.GetSection("TodoList").GetValue<string[]>("Scopes");

    services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration)
            .EnableTokenAcquisitionToCallDownstreamApi(toDoScopes)
            .AddInMemoryTokenCaches();

    services.AddHttpContextAccessor();
    services.AddDownstreamApi("TodoList", builder.Configuration.GetSection("TodoList"));

    services.AddScoped<IFileSystem, FileSystem>();

    _ = services.AddOptions<ApiUsageConfiguration>()
                .Bind(builder.Configuration.GetSection(ApiUsageConfiguration.ConfigurationSectionName))
                .ValidateOnStart();

    _ = services.AddHealthChecks();

    services.AddControllersWithViews(options =>
                                     {
                                         AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                                                                     .RequireAuthenticatedUser()
                                                                     .Build();

                                         options.Filters.Add(new AuthorizeFilter(policy));
                                     }).AddMicrosoftIdentityUI();

    services.AddRazorPages();

    services.AddServerSideBlazor()
            .AddMicrosoftIdentityConsentHandler();

    services.AddApplicationInsightsTelemetry(builder.Configuration);

    services.AddSingleton<ITelemetryInitializer>(_ => new CloudRoleNameTelemetryInitializer(applicationName,
                                                                                            builder.Configuration
                                                                                                   .GetValue<string>("ApplicationInsights:InstrumentationKey")!));

    services.AddApplicationServices(builder.Configuration);

    _ = services.AddScoped<PaginationService>();
    _ = services.AddScoped<SearchFilesServiceData>();
    _ = services.AddScoped<SearchFilesService>();

    builder.Services
           .AddBlazorise( options => { options.Immediate = true; } )
           .AddBootstrap5Providers()
           .AddFontAwesomeIcons();

    WebApplication app = builder.Build();

    ConfigureApplication(app);
    Log.Information("Starting {AppName}", applicationName);

    app.UseExceptionHandler("/Error", true);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Fatal error occurred in {AppName}", applicationName);
}
finally
{
    Log.Information("Stopping {AppName}", applicationName);
    Log.CloseAndFlush();
}

return;

void ConfigureApplication(WebApplication webApplication)
{
    webApplication.UseHttpsRedirection();
    webApplication.UseStaticFiles();

    webApplication.UseRouting();

    webApplication.UseAuthentication();
    webApplication.UseAuthorization();

    webApplication.MapControllers();
    webApplication.MapBlazorHub();
    webApplication.MapFallbackToPage("/_Host");
}

namespace AStar.Dev.Web
{
    internal class RabbitHealthCheck(IOptions<ApiUsageConfiguration> usageConfiguration)
    {
        public async Task<IConnection> CreateConnection()
        {
            ApiUsageConfiguration config  = usageConfiguration.Value;
            var                   factory = new ConnectionFactory { HostName = config.HostName, UserName = config.UserName, Password = config.Password, };

            return await factory.CreateConnectionAsync();
        }
    }
}

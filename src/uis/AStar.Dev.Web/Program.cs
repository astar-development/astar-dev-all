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
    _ = builder.AddSerilogLogging(Configuration.ExternalSettingsFile);

    Log.Information("Starting {AppName}", applicationName);

    IServiceCollection services = builder.Services;

    _ = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);

    string[]? toDoScopes = builder.Configuration.GetSection("TodoList").GetValue<string[]>("Scopes");

    _ = services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration)
            .EnableTokenAcquisitionToCallDownstreamApi(toDoScopes)
            .AddInMemoryTokenCaches();

    _ = services.AddHttpContextAccessor();
    _ = services.AddDownstreamApi("TodoList", builder.Configuration.GetSection("TodoList"));

    _ = services.AddScoped<IFileSystem, FileSystem>();

    _ = services.AddOptions<ApiUsageConfiguration>()
                .Bind(builder.Configuration.GetSection(ApiUsageConfiguration.ConfigurationSectionName))
                .ValidateOnStart();

    _ = services.AddHealthChecks();

    _ = services.AddControllersWithViews(options =>
                                     {
                                         AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                                                                     .RequireAuthenticatedUser()
                                                                     .Build();

                                         options.Filters.Add(new AuthorizeFilter(policy));
                                     }).AddMicrosoftIdentityUI();

    _ = services.AddRazorPages();

    _ = services.AddServerSideBlazor()
            .AddMicrosoftIdentityConsentHandler();

    _ = services.AddApplicationInsightsTelemetry(builder.Configuration);

    _ = services.AddSingleton<ITelemetryInitializer>(_ => new CloudRoleNameTelemetryInitializer(applicationName,
                                                                                            builder.Configuration
                                                                                                   .GetValue<string>("ApplicationInsights:InstrumentationKey")!));

    _ = services.AddApplicationServices(builder.Configuration);

    _ = services.AddScoped<PaginationService>();
    _ = services.AddScoped<SearchFilesServiceData>();
    _ = services.AddScoped<SearchFilesService>();

    _ = builder.Services
           .AddBlazorise(options => { options.Immediate = true; })
           .AddBootstrap5Providers()
           .AddFontAwesomeIcons();

    WebApplication app = builder.Build();

    ConfigureApplication(app);
    Log.Information("Starting {AppName}", applicationName);

    _ = app.UseExceptionHandler("/Error", true);
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

static void ConfigureApplication(WebApplication webApplication)
{
    _ = webApplication.UseHttpsRedirection();
    _ = webApplication.UseStaticFiles();

    _ = webApplication.UseRouting();

    _ = webApplication.UseAuthentication();
    _ = webApplication.UseAuthorization();

    _ = webApplication.MapControllers();
    _ = webApplication.MapBlazorHub();
    _ = webApplication.MapFallbackToPage("/_Host");
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

using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using AStar.Dev.AspNet.Extensions.PipelineExtensions;
using AStar.Dev.AspNet.Extensions.RootEndpoint;
using AStar.Dev.AspNet.Extensions.ServiceCollectionExtensions;
using AStar.Dev.AspNet.Extensions.WebApplicationBuilderExtensions;
using AStar.Dev.Auth.Extensions;
using AStar.Dev.Infrastructure.UsageDb.Data;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.Minimal.Api.Extensions;
using AStar.Dev.Usage.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

string applicationName = typeof(IAssemblyMarker).Assembly.GetName().Name!;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    _ = builder
       .DisableServerHeader()
       .AddSerilogLogging(Configuration.ExternalSettingsFile)
       .Services.AddApiConfiguration(builder.Configuration);

    Log.Information("Starting {AppName}", applicationName);
    IServiceCollection services = builder.Services;
    builder.Services.AddOpenApi();

    builder.Services
           .AddOptions<ApiUsageConfiguration>()
           .Bind(builder.Configuration.GetSection(ApiUsageConfiguration.ConfigurationSectionName));

    string? connectionString = builder.Configuration.GetConnectionString("sqlServer");

    _ = builder.Services.AddScoped<JwtEvents>();
    _ = builder.Services.AddSingleton(_ => new ApiUsageContext(new() { Value = connectionString!, }));
    builder.Services.AddTransient<IPeriodicTimer, SecondsPeriodicTimer>();
    builder.Services.Configure<HostOptions>(options => { options.ServicesStartConcurrently = true; });
    builder.Services.AddHostedService<ProcessUsageEventsService>();

    services.AddUsageServices(builder.Configuration, typeof(IAssemblyMarker).Assembly);

    #pragma warning disable ASP0000
    ServiceProvider buildServiceProvider = services.BuildServiceProvider();
    Send            send                 = buildServiceProvider.GetRequiredService<Send>();
    JwtEvents       events               = buildServiceProvider.GetRequiredService<JwtEvents>();
    #pragma warning restore ASP0000

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer("Bearer", jwtOptions =>
                                   {
                                       jwtOptions.MetadataAddress = "https://login.microsoftonline.com/bb7d94aa-36a9-4a59-a0c1-54a757c47ddf/v2.0/.well-known/openid-configuration";

                                       jwtOptions.TokenValidationParameters = new()
                                                                              {
                                                                                  ValidIssuer      = "https://sts.windows.net/bb7d94aa-36a9-4a59-a0c1-54a757c47ddf/",
                                                                                  ValidateIssuer   = true,
                                                                                  ValidateAudience = true,
                                                                                  ValidAudiences =
                                                                                  [
                                                                                      "api://11cbc21c-c65d-436e-951e-6b3158357be6",
                                                                                      "api://2ca26585-5929-4aae-86a7-a00c3fc2d061",
                                                                                  ],
                                                                                  ValidateIssuerSigningKey = true,
                                                                                  ValidateLifetime         = true,
                                                                                  ClockSkew                = TimeSpan.FromMinutes(3),
                                                                              };

                                       jwtOptions.MapInboundClaims = false;
                                       jwtOptions.Validate();
                                       jwtOptions.Events = events.AddJwtEvents(send, applicationName);
                                   });

    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

    services.Configure<JsonOptions>(options =>
                                    {
                                        options.SerializerOptions.ReferenceHandler            = ReferenceHandler.IgnoreCycles;
                                        options.SerializerOptions.PropertyNameCaseInsensitive = true;
                                    });

    services.AddAuthorization();
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    WebApplication app = builder.Build()
                                .UseApiServices();

    app.ConfigureRootPage(applicationName.Replace(".", " "))
       .UseMetrics();

    app.AddApplicationEndpoints(Assembly.GetExecutingAssembly());

    app.MapGet("/process-events",
               async ([FromServices] ProcessUsageEventsService process, CancellationToken stoppingToken) =>
               {
                   await process.ProcessEventsAsync(stoppingToken);

                   return Results.Ok();
               })
       .WithName("ProcessEvents");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Fatal error occurred in {AppName}", applicationName);
}
finally
{
    Log.CloseAndFlush();
}

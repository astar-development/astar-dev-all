﻿using AStar.Dev.Infrastructure.AdminDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Admin.Api;

public sealed class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder
           .ConfigureTestServices(services =>
                                  {
                                      ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AdminContext>));

                                      services.Remove(dbContextDescriptor!);

                                      services.AddScoped<AdminContext>(_ => new(
                                                                                $"Server=sql1;Database=AdminDb-{Guid.CreateVersion7()};User Id=sa;Password=E&IY!Q65fcMA6J$x;TrustServerCertificate=true;",
                                                                                new()));
                                  });

        _ = builder.UseEnvironment("Development");
    }
}

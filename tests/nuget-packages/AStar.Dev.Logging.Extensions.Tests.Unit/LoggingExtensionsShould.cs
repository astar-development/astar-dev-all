using AStar.Dev.Logging.Extensions.Models;
using AStar.Dev.Utilities;
using Microsoft.AspNetCore.Builder;

namespace AStar.Dev.Logging.Extensions;

public sealed class LoggingExtensionsShould
{
    [Theory]
    [InlineData("This is not a valid filename for a lot of reasons")]
    [InlineData(@"c:\This is not a valid filename\as the path\and filename\do not exist.what.did.you.expect.lol")]
    public void ThrowExceptionWhenAddSerilogLoggingIsCalledButConfigIsntValid(string? fileNameWithPath)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        Action action = () => builder.AddSerilogLogging(fileNameWithPath!);

        _ = action.ShouldThrow<Exception>();
    }

    [Fact(Skip = "Doesn't work...")]
    public void AddTheExpectedNumberOfSerilogServices()
    {
        WebApplicationBuilder builder              = WebApplication.CreateBuilder();
        var                   expectedServiceCount = 147;
        var                   testConfig           = new SerilogConfig();
        testConfig.Serilog.WriteTo = [new() { Args = new() { ServerUrl = "https://example.com", }, },];
        File.WriteAllText("serilig.config", testConfig.ToJson()); // OK, not a true unit test but...

        WebApplicationBuilder sut = builder.AddSerilogLogging("serilig.config");

        sut.Services.Count(d => d.ServiceType.FullName?.StartsWith("Serilog") == false).ShouldBe(expectedServiceCount);
    }
}

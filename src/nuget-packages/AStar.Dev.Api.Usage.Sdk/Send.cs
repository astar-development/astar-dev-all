using System.Text;
using AStar.Dev.Utilities;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AStar.Dev.Api.Usage.Sdk;

/// <summary>
/// </summary>
public sealed class Send(IOptions<ApiUsageConfiguration> usageConfiguration)
{
    /// <summary>
    /// </summary>
    /// <param name="usageEvent"></param>
    /// <param name="cancellationToken"></param>
    public async Task SendUsageEventAsync(ApiUsageEvent usageEvent, CancellationToken cancellationToken)
    {
        try
        {
            ApiUsageConfiguration config = usageConfiguration.Value;

            var                     factory    = new ConnectionFactory { HostName = config.HostName, UserName = config.UserName, Password = config.Password, };
            await using IConnection connection = await factory.CreateConnectionAsync(cancellationToken);
            await using IChannel    channel    = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(config.QueueName, true, false, false, cancellationToken: cancellationToken);

            byte[] body = Encoding.UTF8.GetBytes(usageEvent.ToJson());

            await channel.BasicPublishAsync(string.Empty, config.QueueName, body, cancellationToken);
        }
        catch
        {
        }
    }
}

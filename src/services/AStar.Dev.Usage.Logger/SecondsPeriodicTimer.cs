namespace AStar.Dev.Usage.Logger;

public sealed class SecondsPeriodicTimer : IPeriodicTimer
{
    private readonly PeriodicTimer timer = new(TimeSpan.FromSeconds(1));

    public async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default) =>
        await timer.WaitForNextTickAsync(cancellationToken);

    public void Dispose() =>
        timer.Dispose();
}

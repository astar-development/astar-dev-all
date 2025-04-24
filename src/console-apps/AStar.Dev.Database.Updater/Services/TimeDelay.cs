using System.Globalization;

namespace AStar.Dev.Database.Updater.Services;

internal static class TimeDelay
{
    public static TimeSpan CalculateDelayToNextRun(string targetTime)
    {
        TimeSpan duration = DateTime.Parse(targetTime, CultureInfo.CurrentCulture).Subtract(DateTime.Now);

        if (duration < TimeSpan.Zero)
        {
            duration = duration.Add(TimeSpan.FromHours(24));
        }

        return duration;
    }
}

using AStar.Dev.Infrastructure.AdminDb.Models;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Infrastructure.AdminDb.SeedData;

/// <summary>
///     The <see cref="ScrapeDirectoryData" /> adds the default Scrape Directory configuration when no entry exists
/// </summary>
public static class ScrapeDirectoryData
{
    /// <summary>
    ///     The seed method to populate the database with
    /// </summary>
    /// <param name="context">The <see cref="DbContext" /> to populate</param>
    public static void Seed(DbContext context)
    {
        DbSet<ScrapeDirectory> scrapeDirectories = context.Set<ScrapeDirectory>();

        if (scrapeDirectories.Any())
        {
            return;
        }

        context.Set<ScrapeDirectory>()
               .Add(new()
                    {
                        RootDirectory       = @"C:\Users\jason_17jx22b",
                        BaseSaveDirectory   = @"C:\Users\jason\OneDrive\Pictures\Wallpapers\",
                        BaseDirectory       = @"C:\Users\jason\OneDrive\Pictures\Wallpapers\Wallhaven\",
                        BaseDirectoryFamous = @"C:\Users\jason\OneDrive\Pictures\Famous\",
                        SubDirectoryName    = "New-Subscription-Wallpapers",
                    });
    }
}

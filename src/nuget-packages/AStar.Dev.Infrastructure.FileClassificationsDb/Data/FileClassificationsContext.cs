using AStar.Dev.Infrastructure.Data;
using AStar.Dev.Infrastructure.FileClassificationsDb.Models;
using AStar.Dev.Technical.Debt.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Infrastructure.FileClassificationsDb.Data;

/// <summary>
/// </summary>
public class FileClassificationsContext : DbContext
{
    private readonly AStarDbContextOptions astarDbContextOptions;
    private readonly ConnectionString      connectionString;

    /// <summary>
    ///     Alternative constructor used when creating migrations, the connection string is hard-coded
    /// </summary>
    [Refactor(1, 1, "do we still need this after recent refactorings")]
    public FileClassificationsContext()
        : this(
               new() { Value = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FileConnectionsDb", },
               new())
    {
    }

    /// <summary>
    ///     The main constructor
    /// </summary>
    /// <param name="connectionString">
    ///     An instance of <see cref="ConnectionString" /> used to define the Connection String in a strongly typed way
    /// </param>
    /// <param name="astarDbContextOptions">
    ///     An instance of <see cref="AStarDbContextOptions" /> used to control additional settings - such as Logging
    /// </param>
    public FileClassificationsContext(ConnectionString connectionString, AStarDbContextOptions astarDbContextOptions)
    {
        this.connectionString      = connectionString;
        this.astarDbContextOptions = astarDbContextOptions;
    }

    /// <summary>
    ///     The list of FileClassifications in the dB
    /// </summary>
    public DbSet<FileToFileClassificationMapping> FileClassifications { get; set; } = null!;

    /// <summary>
    ///     The list of FileDetailClassification in the dB
    /// </summary>
    public DbSet<FileDetailClassification> FileDetailClassifications { get; set; } = null!;

    /// <summary>
    ///     The overridden OnModelCreating method
    /// </summary>
    /// <param name="modelBuilder">
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = modelBuilder.HasDefaultSchema(Constants.SchemaName);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileClassificationsContext).Assembly);
    }

    /// <summary>
    ///     The overridden OnConfiguring method
    /// </summary>
    /// <param name="optionsBuilder">
    ///     An instance of <see cref="DbContextOptionsBuilder" /> used to set the configuration on
    /// </param>
    [Refactor(1, 1, "We specify the Migrations Assembly but, I don't believe it exists in this solution")]
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder.UseSqlServer(connectionString.Value

                                        //x => x.MigrationsAssembly("AStar.Infrastructure.Migrations")
                                       );

        if (astarDbContextOptions.EnableLogging)
        {
            _ = optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
            _ = optionsBuilder.LogTo(Console.WriteLine);

            if (astarDbContextOptions.IncludeSensitiveData)
            {
                _ = optionsBuilder.EnableSensitiveDataLogging();
            }
        }
        else
        {
            _ = optionsBuilder.UseLoggerFactory(CreateEmptyLoggerFactory());
        }
    }

    private static ILoggerFactory CreateEmptyLoggerFactory()
        => LoggerFactory.Create(builder => builder.AddFilter((_, _) => false));

    private static ILoggerFactory CreateLoggerFactory()
        => LoggerFactory.Create(static builder => builder
                                .AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information));
}

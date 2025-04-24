using System.IO.Abstractions.TestingHelpers;
using AStar.Dev.Database.Updater.Fixtures;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace AStar.Dev.Database.Updater.Services;

public sealed class FilesServiceShould
{
    private readonly FilesContext context;
    private readonly FilesService sut;

    public FilesServiceShould()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                                            {
                                                { @"c:\temp\.editorconfig2", new("Testing is meh.") },
                                                { @"c:\temp\files.list.txt", new("Testing is meh.") },
                                                { @"c:\demo\jQuery.js", new("some js") },
                                                { @"c:\demo\9.JPG", new([0x12, 0x34, 0x56, 0xd2,]) },
                                                { @"c:\demo\A-0005.JPG", new([0x12, 0x34, 0x56, 0xd2,]) },
                                            });

        context = new FilesContextFixture().Sut;
        sut     = new(context, fileSystem, NullLogger<FilesService>.Instance);
    }

    [Fact(Skip = "Underlying code is broken")]
    public async Task MarkAllSoftDeletePendingAsDeletedOnceComplete()
    {
        IQueryable<FileAccessDetail> f = context.FileAccessDetails.Where(fileAccessDetail => fileAccessDetail.SoftDeletePending);

        context.FileAccessDetails.Count(fileAccessDetail => fileAccessDetail.SoftDeletePending).ShouldBeGreaterThan(0);

        await sut.DeleteFilesMarkedForSoftDeletionAsync(CancellationToken.None);

        context.FileAccessDetails.Count(fileAccessDetail => fileAccessDetail.SoftDeletePending).ShouldBe(0);
        f.Count().ShouldBe(0);
    }

    [Fact(Skip = "Underlying code is broken")]
    public async Task MarkAllHardDeletePendingAsDeletedOnceComplete()
    {
        context.FileAccessDetails.Count(fileAccessDetail => fileAccessDetail.HardDeletePending).ShouldBeGreaterThan(0);

        await sut.DeleteFilesMarkedForHardDeletionAsync(CancellationToken.None);

        context.FileAccessDetails.Count(fileAccessDetail => fileAccessDetail.HardDeletePending).ShouldBe(0);
    }

    [Theory(Skip = "Underlying code is broken")]
    [InlineData("*.*",  true,  5)]
    [InlineData("*.*",  false, 0)]
    [InlineData("*.js", true,  1)]
    [InlineData("*.js", false, 0)]
    public void ReturnTheExpectedFiles(string searchPattern, bool recursive, int expectedCount)
    {
        IEnumerable<string> files = sut.GetFilesFromDirectory(@"c:\", searchPattern, recursive);

        files.Count().ShouldBe(expectedCount);
    }

    [Fact(Skip = "Underlying code is broken")]
    public async Task ProcessNewFilesCorrectly()
    {
        int originalFileCount              = context.Files.Count();
        int originalFileAccessDetailsCount = context.FileAccessDetails.Count();

        await sut.ProcessNewFiles([@"c:\myFile.txt", @"c:\demo\jQuery.js", @"c:\demo\image.gif",],
                                  CancellationToken.None);

        context.Files.Count().ShouldBeGreaterThan(originalFileCount, "the SkiaSharp code fails... will extract shortly");

        context.FileAccessDetails.Count().ShouldBeGreaterThan(originalFileAccessDetailsCount,
                                                              "the SkiaSharp code fails... will extract shortly");
    }

    [Fact(Skip = "Underlying code is broken")]
    public async Task ProcessMovedFilesCorrectly()
    {
        int originalMaxId = context.Files.Max(file => file.Id);

        await sut.ProcessMovedFiles([@"c:\.editorconfig2", @"c:\demo\9.JPG", @"c:\demo\A-0005.JPG",], [@"c:\",],
                                    CancellationToken.None);

        // using var scope = new AssertionScope();

        FileDetail file1 = context.Files.OrderBy(file => file.Id)
                                  .Last(file => file.FileName == ".editorconfig2" && file.DirectoryName == @"c:");

        file1.Id.ShouldNotBe(originalMaxId);
        context.FileAccessDetails.Count(fileAccessDetail => fileAccessDetail.Id == file1.Id).ShouldBe(1);

        FileDetail file2 = context.Files.OrderBy(file => file.Id)
                                  .Last(file => file.FileName == "9.JPG" && file.DirectoryName == @"c:\demo");

        file2.Id.ShouldNotBe(originalMaxId);
        context.FileAccessDetails.Count(fileAccessDetail => fileAccessDetail.Id == file2.Id).ShouldBe(1);

        FileDetail file3 = context.Files.OrderBy(file => file.Id)
                                  .Last(file => file.FileName == "A-0005.JPG" && file.DirectoryName == @"c:\demo");

        file3.Id.ShouldNotBe(originalMaxId);
        context.FileAccessDetails.Count(fileAccessDetail => fileAccessDetail.Id == file3.Id).ShouldBe(1);
    }

    [Fact(Skip = "Underlying code is broken")]
    public async Task RemoveFilesFromDbThatDoNotExistAnyMoreCorrectly()
    {
        int originalFileCount              = context.Files.Count();
        int originalFileAccessDetailsCount = context.FileAccessDetails.Count();

        await sut.RemoveFilesFromDbThatDoNotExistAnyMore([@"c:\not.important.txt",], CancellationToken.None);

        // using var scope = new AssertionScope();

        context.Files.Count().ShouldBeLessThan(originalFileCount, "the SkiaSharp code fails... will extract shortly");

        context.FileAccessDetails.Count().ShouldBeLessThan(originalFileAccessDetailsCount,
                                                           "the SkiaSharp code fails... will extract shortly");
    }
}

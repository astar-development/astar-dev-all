using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Random.Image.Selector.Config;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Random.Image.Selector;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private          int                   fileCount;
    public required  FilesContext          FilesContext;
    private          string                selectedImage = string.Empty;

    public MainWindow()
    {
        InitializeComponent();

        try
        {
            CancellationTokenSource                     =  new();
            DispatcherTimer                             =  new();
            DispatcherTimer.Tick                        += DispatcherTimer_Tick!;
            DispatcherTimer.Interval                    =  new(0, 0, 5);
            Application.Current.MainWindow!.WindowState =  WindowState.Maximized;
            selectedImage                               =  string.Empty;
            string appSettings = File.ReadAllText("appsettings.json");
            FileDetail.Content = appSettings;
            AppSettings appSettings1 = JsonSerializer.Deserialize<AppSettings>(appSettings, jsonSerializerOptions)!;

            FileDetail.Content = string.IsNullOrWhiteSpace(appSettings1.ConnectionStrings.SqlServer)
                                     ? "Oops"
                                     : appSettings1.ConnectionStrings.SqlServer;

            FilesContext = new(new() { Value = appSettings1.ConnectionStrings.SqlServer, },
                               new());
        }
        catch (Exception ex)
        {
            File.AppendAllText(@"c:\logs\astar-random.image.log.txt",
                               $"UTC: {DateTime.UtcNow} - {ex.Message} (MainWindow){Environment.NewLine}");
        }
    }

    private IList<string> Files { get; set; } = [];

    private DispatcherTimer DispatcherTimer { get; } = new();

    private CancellationTokenSource CancellationTokenSource { get; set; } = new();

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            CancellationTokenSource = new();
            StartSearch.IsEnabled   = false;
            CancelSearch.IsEnabled  = true;
            SelectImageForImageSource();
            DispatcherTimer.Start();
        }
        catch (Exception ex)
        {
            File.AppendAllText(@"c:\logs\astar-random.image.log.txt",
                               $"UTC: {DateTime.UtcNow} - {ex.Message} (Button_Click){Environment.NewLine}");
        }
    }

    private void SelectImageForImageSource()
    {
        try
        {
            string file;
            FileDetail.Content = $"Starting search at {DateTime.Now}...";

            // case-insensitive PLEASE!!! LOL
            if (StartingFolder.Text != "D:\\Wallhaven")
            {
                FileDetail.Content = $"Starting manual search in the {StartingFolder.Text} directory at {DateTime.Now} ({fileCount})...";
                Files              = Directory.GetFiles(StartingFolder.Text, "*.*", SearchOption.AllDirectories);
                fileCount          = Files.Count;
                int random = RandomNumberGenerator.GetInt32(0, fileCount + 1);
                file = Files.Skip(random).Take(1).First();
            }
            else
            {
                FileDetail.Content = $"Searching files started at {DateTime.Now}...";

                fileCount = FilesContext.Files
                                        .Include(f => f.FileAccessDetail).Where(f => !f.FileAccessDetail.SoftDeleted && !f.FileAccessDetail.HardDeletePending &&
                                                                                     !f.FileAccessDetail.SoftDeletePending)
                                        .Select(f => Path.Combine(f.DirectoryName, f.FileName)).Count();

                int random = RandomNumberGenerator.GetInt32(0, fileCount + 1);

                file = FilesContext.Files.Include(f => f.FileAccessDetail)
                                   .Where(f => !f.FileAccessDetail.SoftDeleted && !f.FileAccessDetail.HardDeletePending &&
                                               !f.FileAccessDetail.SoftDeletePending)
                                   .Select(f => Path.Combine(f.DirectoryName, f.FileName)).ToList().Skip(random).Take(1).First();
            }

            FileDetail.Content  = $"Got {file} at {DateTime.Now}...";
            ImageDisplay.Source = new BitmapImage(new(file));
        }
        catch (Exception ex)
        {
            //Think about how to handle / what message and where
            FileDetail.Content = ex.GetBaseException().Message;
            AddToDeleteFile();
            DispatcherTimer.Stop();
        }
    }

    private void DispatcherTimer_Tick(object sender, EventArgs e) =>
        SelectImageForImageSource();

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        DispatcherTimer.Stop();
        CancellationTokenSource.Cancel();
        StartSearch.IsEnabled  = true;
        CancelSearch.IsEnabled = false;
    }

    private void Button_Click_2(object sender, RoutedEventArgs e) =>
        AddToDeleteFile();

    private void AddToDeleteFile()
    {
        Files = Files.Where(file => file != selectedImage).ToList();

        File.AppendAllText(@"c:\temp\delete.json",
                           $"UTC: {DateTime.UtcNow}{Environment.NewLine}{selectedImage}{Environment.NewLine}");

        File.AppendAllText(@"c:\temp\fileCount.txt",
                           $"UTC: {DateTime.UtcNow}{Environment.NewLine}{Files.Count()}{Environment.NewLine}");

        selectedImage = string.Empty;
        SelectImageForImageSource();
    }
}

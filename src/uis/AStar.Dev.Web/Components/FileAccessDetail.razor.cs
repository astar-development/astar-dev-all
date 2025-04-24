using System.Diagnostics;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AStar.Dev.Web.Components;

public partial class FileAccessDetail
{
    public string DeletionStatus { get; private set; } = string.Empty;

    // [Parameter]
    // public FileDetail FileDetail { get; set; } = new();

    [Parameter]
    public string RootDirectory { get; set; } = string.Empty;

    // [Inject]
    // public required FilesApiClient FilesApiClient { get; set; }

    [Inject]
    public required IJSRuntime JsRuntime { get; set; }

    [Inject]
    public required NavigationManager Navigation { get; set; }

    private async Task OnButtonClicked()
    {
        try
        {
            //_ = Process.Start("explorer.exe", FileDetail.FullNameWithPath);
        }
        catch
        {
            await OpenImageAsync();
        }
    }

    private async Task OpenImageAsync()
    {
        await Task.CompletedTask;
        try
        {
            // var url = Navigation.ToAbsoluteUri($"/image-display?fullNameWithPath={HttpUtility.UrlEncode(FileDetail.FullNameWithPath)}&width={FileDetail.Width}").ToString();
            // await OpenUrlInNewTab(url);
        }
        catch
        {
            // NAR
        }
    }

    private async Task OpenUrlInNewTab(string url)
    {
        _ = await JsRuntime.InvokeAsync<object>("open", url, "_blank");
    }

    private async Task MarkForSoftDeleteImageAsync(int fileId)
    {
        await Task.CompletedTask;
        string result = string.Empty; // await FilesApiClient.MarkForSoftDeletionAsync(fileId);

        if (result == "Marked for soft deletion")
        {
            //FileDetail.FileAccessDetail.SoftDeletePending = true;
        }

        DeletionStatus = result;
    }

    private async Task UnMarkForSoftDeleteImageAsync(int fileId)
    {
        await Task.CompletedTask;
        // string result = await FilesApiClient.UndoMarkForSoftDeletionAsync(fileId);
        //
        // if (result == "Mark for soft deletion has been undone")
        // {
        //     FileDetail.FileAccessDetail.SoftDeletePending = false;
        // }
        //
        // DeletionStatus = result;
    }

    private async Task MarkForHardDeleteImageAsync(int fileId)
    {
        await Task.CompletedTask;
        // string result = await FilesApiClient.MarkForHardDeletionAsync(fileId);
        //
        // if (result == "Marked for hard deletion.")
        // {
        //     FileDetail.FileAccessDetail.HardDeletePending = true;
        // }
        //
        // DeletionStatus = result;
    }

    private async Task UnMarkForHardDeleteImageAsync(int fileId)
    {
        await Task.CompletedTask;
        // string result = await FilesApiClient.UndoMarkForHardDeletionAsync(fileId);
        //
        // if (result == "Mark for hard deletion has been undone")
        // {
        //     FileDetail.FileAccessDetail.HardDeletePending = false;
        // }
        //
        // DeletionStatus = result;
    }

    private async Task MarkForMovingAsync(int fileId)
    {
        await Task.CompletedTask;
    //     string result = await FilesApiClient.MarkForMovingAsync(fileId);
    //
    //     if (result == "Mark for moving was successful")
    //     {
    //         FileDetail.FileAccessDetail.MoveRequired = true;
    //     }
    //
    //     DeletionStatus = result;
    }

    private async Task UndoMarkForMovingAsync(int fileId)
    {
        await Task.CompletedTask;
        // string result = await FilesApiClient.UndoMarkForMovingAsync(fileId);
        //
        // if (result == "Undo mark for moving was successful")
        // {
        //     FileDetail.FileAccessDetail.MoveRequired = false;
        // }
        //
        // DeletionStatus = result;
    }
}

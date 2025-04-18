using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace FalandoSobre.Web.Components.Pages.CreateReport;

public class CreateReportPage : ComponentBase
{
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

    [Inject] protected NavigationManager? Navi { get; set; }

    [Inject] public IReportRepository? ReportRepository { get; set; } = null!;

    protected Report Model { get; set; } = new();

    protected bool uploadInProgress = false;
    protected string successMessage = string.Empty;
    protected string errorMessage = string.Empty;
    private List<IBrowserFile> selectedFiles = new List<IBrowserFile>();
    private List<string> imagePreviewUrls = new List<string>();

    [Inject]
    public required ILogger<CreateReportPage> Logger { get; set; }


    protected async Task CreateReport()
    {
        uploadInProgress = true;
        successMessage = string.Empty;
        errorMessage = string.Empty;

        var authState = await AuthStateProvider.GetAuthenticationStateAsync();

        var user = authState.User;

        try
        {
            Model.UserName = user.Identity?.Name;
            var data = await ReportRepository!.AddAsync(Model);
            //await UploadImages();
            successMessage = "Publicação criada com sucesso!";
            imagePreviewUrls.Clear();
            //Navi!.NavigateTo("/");

        }
        catch (Exception ex)
        {
            Logger!.LogInformation($"Erro ao criar publicação: {ex.InnerException?.Message ?? ex.Message}");
            errorMessage = $"{ex.InnerException?.Message ?? ex.Message}";
            //Navi!.NavigateTo("/create");
        }
        //finally
        //{
        //    uploadInProgress = false;
        //    selectedFiles.Clear();
        //    StateHasChanged();
        //}

    }
}

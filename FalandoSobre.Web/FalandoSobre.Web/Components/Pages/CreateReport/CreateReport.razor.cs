using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;

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

        if (user is null || !user.Identity?.IsAuthenticated == true)
        {
            throw new InvalidOperationException("Usuário não autenticado");
        }

        try
        {
            Model.UserName = user.Identity?.Name;
            Logger!.LogInformation("Model report name: {Model}", Model.ReportName);
            Logger!.LogInformation("Model report description: {Model}", Model.ReportDescription);
            Logger!.LogInformation("Model report type: {Model}", Model.TypeReport);
            Logger!.LogInformation("Model report UserName: {Model}", Model.UserName);
            var teste = await ReportRepository!.AddAsync(Model);
            Logger!.LogInformation("TESTE CRIADO COM SUCESO: {teste.Id}", teste.Id);
            //await UploadImages();
            successMessage = "Publicação criada com sucesso!";
            imagePreviewUrls.Clear();
            //Navi!.NavigateTo("/");

        }
        catch (Exception ex)
        {
            Logger!.LogInformation("teste.UserName: {UserName}");
            errorMessage = $"Erro ao criar publicação: {ex.InnerException?.Message ?? ex.Message}";
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

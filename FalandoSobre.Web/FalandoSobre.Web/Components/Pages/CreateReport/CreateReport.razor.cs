using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FalandoSobre.Web.Components.Pages.CreateReport;

public class CreateReportPage : ComponentBase
{
    [Inject] protected NavigationManager? Navi { get; set; }

    [Inject] public IReportRepository? ReportRepository { get; set; }

    protected Report Model { get; set; } = new Report();

    protected bool uploadInProgress = false;
    protected string successMessage = string.Empty;
    protected string errorMessage = string.Empty;
    private List<IBrowserFile> selectedFiles = new List<IBrowserFile>();
    private List<string> imagePreviewUrls = new List<string>();

    protected async Task CreateReport()
    {
        uploadInProgress = true;
        successMessage = string.Empty;
        errorMessage = string.Empty;

        try
        {
            var teste = await ReportRepository!.AddAsync(Model);
            Console.WriteLine("teste.UserName aquiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii" + teste.UserName);
            //await UploadImages();
            successMessage = "Publicação criada com sucesso!";
            imagePreviewUrls.Clear();
            //Navi!.NavigateTo("/");

        }
        catch (Exception ex)
        {
            errorMessage = $"Erro ao criar publicação: {ex.InnerException?.Message ?? ex.Message}";
            //Navi!.NavigateTo("/create");
        }
        finally
        {
            uploadInProgress = false;
            selectedFiles.Clear();
            StateHasChanged();
        }

    }
}

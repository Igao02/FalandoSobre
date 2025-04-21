using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;

namespace FalandoSobre.Web.Components.Pages.CreateReport;

public class CreateReportPage : ComponentBase
{
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

    [Inject] protected NavigationManager? Navi { get; set; }

    [Inject] public IReportRepository? ReportRepository { get; set; } = null!;

    [Inject] public IImageRepository? ImageRepository { get; set; } = null!;

    protected Report Model { get; set; } = new();

    protected Image ImageModel { get; set; } = new();

    protected string successMessage = string.Empty;
    protected string errorMessage = string.Empty;
    protected List<IBrowserFile> selectedFiles = new List<IBrowserFile>();
    protected List<string> imagePreviewUrls = new List<string>();
    protected bool uploadInProgress = false;

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
            //Model.Id = Guid.Empty;
            var data = await ReportRepository!.AddAsync(Model);
            Model.Id = data.Id;;
            successMessage = "Publicação criada com sucesso!";
            await UploadImages();
            imagePreviewUrls.Clear();
            //Navi!.NavigateTo("/");

        }
        catch (Exception ex)
        {
            Logger!.LogInformation($"Erro ao criar publicação: {ex.InnerException?.Message ?? ex.Message}");
            uploadInProgress = false;
            errorMessage = $"{ex.InnerException?.Message ?? ex.Message}";
            //Navi!.NavigateTo("/create");
        }
        finally
        {
            uploadInProgress = false;
            selectedFiles.Clear();
            Model = new();
            ImageModel = new();
            StateHasChanged();
        }

    }

    protected async Task HandleSelectedFiles(InputFileChangeEventArgs e)
    {
        const long MaxFileSize = 10 * 1024 * 1024;
        const int BufferSize = 8192;
        selectedFiles = new List<IBrowserFile>();
        imagePreviewUrls.Clear();

        try
        {
           Logger.LogInformation("Selecionando arquivos...");

            foreach (var file in e.GetMultipleFiles())
            {
                if (file.Size > MaxFileSize)
                {
                    Logger.LogInformation($"O arquivo {file.Name} excede o tamanho permitido de 10 MB.");
                    continue;
                }

                selectedFiles.Add(file);

                using var stream = file.OpenReadStream(maxAllowedSize: MaxFileSize);
                using var memoryStream = new MemoryStream();
                var buffer = new byte[BufferSize];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }

                var imageUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
                imagePreviewUrls.Add(imageUrl);
            }

            Logger.LogInformation("Arquivos processados com sucesso.");
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogInformation($"Erro ao selecionar arquivos: {ex.Message}");
        }
    }

    private async Task UploadImages()
    {
        var maxFileSize = 10 * 1024 * 1024;
        var bufferSize = 8192;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            Logger.LogInformation("Iniciando o upload das imagens...");
            Logger.LogInformation($"Quantidade de arquivos selecionados: {selectedFiles.Count}");

            foreach (var file in selectedFiles)
            {
                if (file.Size > maxFileSize)
                {
                    Logger.LogInformation($"O arquivo {file.Name} excede o tamanho permitido de 10 MB.");
                    continue;
                }

                Logger.LogInformation($"Processando o arquivo: {file.Name}, Tamanho: {file.Size} bytes");

                using var stream = file.OpenReadStream(maxAllowedSize: maxFileSize);
                using var memoryStream = new MemoryStream();
                var buffer = new byte[bufferSize];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }

                var fileBytes = memoryStream.ToArray();

                var fileExtension = Path.GetExtension(file.Name);
                var fileName = $"{file.Name}";

                var novaImagem = new Image(
                    imageUrl: fileName,
                    conteudoArquivo: fileBytes,
                    imageDate: DateTime.Now,
                    reportId: Model.Id 
                );
                Logger.LogInformation($"Criando nova imagem: {novaImagem}");

                await ImageRepository!.AddImageAsync(novaImagem);
                Logger.LogInformation($"Imagem {file.Name} enviada com sucesso.");
            }
        }
        catch (Exception ex)
        {
            Logger.LogInformation($"Erro ao fazer upload das imagens: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            Logger.LogInformation($"Tempo total para upload: {stopwatch.ElapsedMilliseconds} ms");
        }
    }


}

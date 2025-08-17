using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.UseCases.ImageUseCase.CreateUseCase;
using FalandoSobreApplication.UseCases.ImageUseCase.ListByReportId;

namespace FalandoSobre.Web.Handlers;

public class ImageHandler(IHttpClientFactory httpClientFactory, ILogger<ReportHandler> logger)
    : IImageRepository
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    public async Task<Image> AddImageAsync(Image image)
    {
        var response = _httpClient.PostAsJsonAsync("/images/create", image);

        if (response.Result.IsSuccessStatusCode)
        {
            var createdResponse = await response.Result.Content.ReadFromJsonAsync<CreateImageResponse>();
            logger.LogInformation("Imagem criada com sucesso: {createdResponse}", createdResponse);

            var createdImage = new Image(
                imageUrl: createdResponse!.ImageUrl,
                conteudoArquivo: null,
                imageDate: createdResponse.ImageDate,
                reportId: createdResponse.ReportId,
                applicationUserId: createdResponse.ApplicationUserId
            )
            {
                Id = createdResponse.Id
            };

            return createdImage!;
        }
        else
        {
            var error = response.Result.Content.ReadAsStringAsync();
            logger.LogError("Erro ao criar a imagem: {Error}", error);
            throw new ApplicationException($"Erro ao criar a imagem: {error}");
        }
    }

    public Task DeleteImageAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Image?> GetImageAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Image>> GetListAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<(Guid Id, string ImageUrl, Guid? ReportId)>> GetImageByReportId(Guid id)
    {
        var url = $"/images/reports/?id={id}";
        var response = await _httpClient.GetAsync(url);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrWhiteSpace(jsonResponse))
        {
            logger.LogInformation("Resposta recebida: {jsonResponse}", jsonResponse);
        }

        if (!response.IsSuccessStatusCode)
            return Enumerable.Empty<(Guid, string, Guid?)>();

        var imageResponses = await response.Content.ReadFromJsonAsync<List<ImageListByReportIdResponse>>();

        if (imageResponses is null || !imageResponses.Any())
            return Enumerable.Empty<(Guid, string, Guid?)>();

        return imageResponses.Select(img => (
            (Guid Id, string ImageUrl, Guid? ReportId))((
                img.Id,
                img.ImageUrl ?? string.Empty,
                img.ReportId
            )));
    }
}
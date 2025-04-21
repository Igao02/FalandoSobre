using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.UseCases.ImageUseCase.CreateUseCase;

namespace FalandoSobre.Web.Handlers;

public class ImageHandler : IImageRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReportHandler> _logger;

    public ImageHandler(IHttpClientFactory httpClientFactory, ILogger<ReportHandler> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    public async Task<Image> AddImageAsync(Image image)
    {
        var response = _httpClient.PostAsJsonAsync("/images/create", image);

        if (response.Result.IsSuccessStatusCode)
        {
            var createdResponse = await response.Result.Content.ReadFromJsonAsync<CreateImageResponse>();
            _logger.LogInformation("Imagem criada com sucesso: {createdResponse}", createdResponse);

            var createdImage = new Image(
                imageUrl: createdResponse!.ImageUrl,
                conteudoArquivo: null,
                imageDate: createdResponse.ImageDate,
                reportId: createdResponse.ReportId
            )
            {
                Id = createdResponse.Id
            };

            return createdImage!;
        }
        else
        {
            var error = response.Result.Content.ReadAsStringAsync();
            _logger.LogError("Erro ao criar a imagem: {Error}", error);
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
}

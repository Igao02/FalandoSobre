using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;

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

    public Task<Image> AddImageAsync(Image image)
    {
        var response = _httpClient.PostAsJsonAsync("/images/create", image);

        if (response.Result.IsSuccessStatusCode)
        {
            var createdImage = response.Result.Content.ReadFromJsonAsync<Image>();
            _logger.LogInformation("Imagem criada com sucesso: {Image}", createdImage);
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

using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.UseCases.LikeUseCase.Create;

namespace FalandoSobre.Web.Handlers;

public class LikeHandler(IHttpClientFactory httpClientFactory, ILogger<LikeHandler> logger)
    : ILikeRepository
{

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");


    public async Task<Like> AddLikesAsync(Like like)
    {
        var response = _httpClient.PostAsJsonAsync("/create-like", like);

        if (response.Result.IsSuccessStatusCode)
        {
            var createdResponse = await response.Result.Content.ReadFromJsonAsync<LikeResponse>();
            logger.LogInformation("Like criado com sucesso!");

            var createdLike = new Like
            {
                Actived = true,
                ApplicationUserId = createdResponse!.ApplicationUserId,
                Id = createdResponse.Id
            };

            return createdLike;
        }
        else
        {
            var error = response.Result.Content.ReadAsStringAsync();
            logger.LogError("Erro ao criar curtida: {Error}", error);
            throw new ApplicationException($"Erro ao criar curtida: {error}");
        }

    }

    public Task<Like?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Like>> GetLikesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Like?> GetUserLikeAsync(string userName, Guid reportId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Like>> GetUserLikesAsync(string userName)
    {
        throw new NotImplementedException();
    }

    public Task RemoveLikesAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}

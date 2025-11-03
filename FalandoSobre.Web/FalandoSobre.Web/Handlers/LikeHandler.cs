using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.UseCases.LikeUseCase.Create;
using FalandoSobreApplication.UseCases.LikeUseCase.ListByUserId;
using FalandoSobreApplication.UseCases.LikeUseCase.Delete;
using FalandoSobreApplication.UseCases.LikeUseCase.ListByReportId;
using System.Net.Http.Json;

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

    public async Task<IEnumerable<Like>> GetLikesByUserIdAsync(string userId)
    {
        try
        {
            var url = $"/likes/user/{userId}";
            var response = await _httpClient.GetFromJsonAsync<ListLikesByUserIdResponse>(url);
            
            if (response?.Likes == null)
                return new List<Like>();

            return response.Likes.Select(dto => new Like
            {
                Id = dto.Id,
                ReportId = dto.ReportId,
                ApplicationUserId = dto.ApplicationUserId,
                Actived = dto.Actived,
                LikeDate = dto.LikeDate
            });
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Erro ao buscar likes do usuário {UserId}", userId);
            return new List<Like>();
        }
    }

    public async Task<IEnumerable<Like>> GetLikesByReportIdAsync(Guid reportId)
    {
        try
        {
            var url = $"/likes/report/{reportId}";
            var response = await _httpClient.GetFromJsonAsync<ListLikesByReportIdResponse>(url);
            
            if (response?.Likes == null)
                return new List<Like>();

            return response.Likes.Select(dto => new Like
            {
                Id = dto.Id,
                ReportId = reportId,
                ApplicationUserId = dto.ApplicationUserId,
                Actived = true,
                LikeDate = dto.LikeDate
            });
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Erro ao buscar likes do report {ReportId}", reportId);
            return new List<Like>();
        }
    }

    public async Task<Like?> GetLikeByUserAndReportAsync(string userId, Guid reportId)
    {
        try
        {
            var likes = await GetLikesByUserIdAsync(userId);
            return likes.FirstOrDefault(l => l.ReportId == reportId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao buscar like do usuário {UserId} no report {ReportId}", userId, reportId);
            return null;
        }
    }

    public async Task<bool> RemoveLikeAsync(string userId, Guid reportId)
    {
        try
        {
            var url = $"/delete-like?userId={userId}&reportId={reportId}";
            var response = await _httpClient.DeleteAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Like removido com sucesso!");
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                logger.LogError("Erro ao remover curtida: {Error}", error);
                return false;
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Erro ao remover like");
            return false;
        }
    }
}

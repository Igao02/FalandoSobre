using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.UseCases.ReportUseCase.ListUseCase;
using FalandoSobreApplication.UseCases.UserInfoUseCase.Create;
using FalandoSobreApplication.UseCases.UserInfoUseCase.GetByUseId;

namespace FalandoSobre.Web.Handlers;

public class UserInfoHandler(IHttpClientFactory httpClientFactory, ILogger<UserInfoHandler> logger) 
    : IUserInfoRepository
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    public async Task<UserInfo> AddAsync(UserInfo userInfo)
    {
        var response = _httpClient.PostAsJsonAsync("/user-info/create", userInfo);

        if (response.Result.IsSuccessStatusCode)
        {
            var createdResponse = await response.Result.Content.ReadFromJsonAsync<CreateUserInfoResponse>();
            logger.LogInformation("Atualização de usuário criada com sucesso: {createdResponse}", createdResponse);

            var createdUserInfo = new UserInfo
            {
                ProfilePhoto = createdResponse!.ProfilePhoto,
                Actived = createdResponse!.Actived,
                ApplicationUserId = createdResponse!.ApplicationUserId,
                CreatedAt = createdResponse!.CreatedAt,
                Id = createdResponse!.Id,
            };

            return createdUserInfo;
        }
        else
        {
            var error = response.Result.Content.ReadAsStringAsync();
            logger.LogError("Erro ao criar informações adicionais ao usuário: {Error}", error);
            throw new ApplicationException($"Erro ao criar informações adicionais: {error}");
        }
    }

    public Task<IEnumerable<UserInfo?>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<UserInfo?> GetImageByUserId(string userId)
    {
        var url = $"/user-info/user/?applicationUserId={userId}";
        var response = await _httpClient.GetAsync(url);
        var jsonResponse = await response.Content.ReadAsStringAsync();

       if(jsonResponse != null)
        {
            logger.LogInformation("Imagem recebida com sucesso: {jsonResponse}", jsonResponse);
        }

        if (!response.IsSuccessStatusCode) return null;

        var imageResponse = await response.Content.ReadFromJsonAsync<GetByUserIdResponse>();
        
        if(imageResponse is null)
        {
            logger.LogInformation("Nenhuma informação adicional encontrada para o usuário com ID: {UserId}", userId);
            return null;
        }

        return new UserInfo
        {
            Id = imageResponse.Id,
            ProfilePhoto = imageResponse.ProfilePhoto!,
            ApplicationUserId = imageResponse.ApplicationUserId!,
        };
    }

    public Task<UserInfo> Save(UserInfo userInfo)
    {
        throw new NotImplementedException();
    }
}

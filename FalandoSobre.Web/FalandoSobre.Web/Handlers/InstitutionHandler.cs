using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.UseCases.InstitutionUseCase.Create;


namespace FalandoSobre.Web.Handlers;

public class InstitutionHandler(IHttpClientFactory httpClientFactory, ILogger<InstitutionHandler> logger) 
    : IInstitutionRepository
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    public async Task<Institution> AddAsync(Institution institution)
    {
        var response = await _httpClient.PostAsJsonAsync("/institutions/create", institution);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var createdResponse = await response.Content.ReadFromJsonAsync<CreateInstitutionResponse>();

            var createdInstitution = new Institution(
                createdResponse!.CorporateName,
                createdResponse.Document,
                createdResponse.Cep,
                createdResponse.City,
                createdResponse.Street,
                createdResponse.NumHome,
                createdResponse.Complement,
                createdResponse.CreationDate,
                createdResponse.UserName,
                createdResponse.Neighborhood,
                createdResponse.Uf,
                createdResponse.ApplicationUserId,
                createdResponse.Actived
            )
            {
                Id = createdResponse.Id
            };
            logger.LogInformation("Instituição criada com sucesso: {Institution}", createdInstitution);
            return createdInstitution;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            logger.LogError("Erro ao criar a instituição: {Error}", error);
            throw new ApplicationException($"Erro ao criar a instituição: {error}");
        }
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Institution> EditAsync(Institution institution)
    {
        throw new NotImplementedException();
    }

    public Task<Institution?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Institution?> GetByApplicationUserIdAsync(string applicationUserId)
    {
        throw new NotImplementedException();
    }

    public Task<Institution?> GetByDocAsync(string doc)
    {
        throw new NotImplementedException();
    }

    public Task<Institution?> GetByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Institution>> GetListAsync()
    {
        throw new NotImplementedException();
    }
}

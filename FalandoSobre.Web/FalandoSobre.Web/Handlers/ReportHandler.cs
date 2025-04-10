using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;

namespace FalandoSobre.Web.Handlers;

public class ReportHandler(IHttpClientFactory httpClientFactory) : IReportRepository
{

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    public async Task<Report> AddAsync(Report report)
    {
        var response = await _httpClient.PostAsJsonAsync("/reports/create", report);

        if (response.IsSuccessStatusCode)
        {
            var createdReport = await response.Content.ReadFromJsonAsync<Report>();
            return createdReport!;
        }
        else
        {
            // Você pode lançar uma exceção com detalhes do erro
            var error = await response.Content.ReadAsStringAsync();
            throw new ApplicationException($"Erro ao criar o relatório: {error}");
        }
    }


    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Report> EditAsync(Report report)
    {
        throw new NotImplementedException();
    }

    public Task<Report?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Report>> GetListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Report>> GetReportsByTypeAsync(string type)
    {
        throw new NotImplementedException();
    }
}

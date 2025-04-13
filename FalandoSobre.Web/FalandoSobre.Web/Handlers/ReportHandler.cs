using Azure;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;

namespace FalandoSobre.Web.Handlers;

public class ReportHandler: IReportRepository
{
    private readonly HttpClient _httpClient;
    //private readonly ILogger<ReportHandler> _logger;

    public ReportHandler(IHttpClientFactory httpClientFactory, ILogger<ReportHandler> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        //_logger = logger ?? throw new ArgumentNullException(nameof(logger));  // Verifica se logger é null
    }

    public async Task<Report> AddAsync(Report report)
    {
        //_logger.LogInformation("ReportHandler AddAsync report: {Report}", report);
        var response = await _httpClient.PostAsJsonAsync("/reports/create", report);

        if (response.IsSuccessStatusCode)
        {
            var createdReport = await response.Content.ReadFromJsonAsync<Report>();
            return createdReport!;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            //_logger.LogError("Erro ao criar o relatório: {Error}", error);
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

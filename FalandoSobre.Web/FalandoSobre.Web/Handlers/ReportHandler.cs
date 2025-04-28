using Azure;
using FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;

namespace FalandoSobre.Web.Handlers;

public class ReportHandler: IReportRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReportHandler> _logger;

    public ReportHandler(IHttpClientFactory httpClientFactory, ILogger<ReportHandler> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    public async Task<Report> AddAsync(Report report)
    {
        var response = await _httpClient.PostAsJsonAsync("/reports/create", report);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("JSON recebido da API: {Json} ", jsonResponse);

        if (response.IsSuccessStatusCode)
        {

            var createdResponse = await response.Content.ReadFromJsonAsync<CreateReportResponse>();
            _logger.LogInformation("response depois {response}: ", response);

            var createdReport = new Report(
                createdResponse!.ReportName,
                createdResponse.TypeReport,
                createdResponse.ReportDescription,
                createdResponse.ReportDate,
                createdResponse.UserName,
                createdResponse.IsEvent,
                createdResponse.Actived,
                createdResponse.ApplicationUserId
            )
            {
                Id = createdResponse.Id
            };

            _logger.LogInformation("Relatório criado com sucesso: {Report}", createdReport);
            return createdReport;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Erro ao criar o relatório: {Error}", error);
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

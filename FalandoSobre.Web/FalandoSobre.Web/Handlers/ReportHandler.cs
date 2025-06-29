using FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;
using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Dto.PagedResponse;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.UseCases.ReportUseCase.ListUseCase;
using System.Text.Json;

namespace FalandoSobre.Web.Handlers;

public class ReportHandler(IHttpClientFactory httpClientFactory, ILogger<ReportHandler> logger)
    : IReportRepository
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    public async Task<Report> AddAsync(Report report)
    {
        var response = await _httpClient.PostAsJsonAsync("/reports/create", report);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        logger.LogInformation("JSON recebido da API: {Json} ", jsonResponse);

        if (response.IsSuccessStatusCode)
        {

            var createdResponse = await response.Content.ReadFromJsonAsync<CreateReportResponse>();

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

            logger.LogInformation("Relatório criado com sucesso: {Report}", createdReport);
            return createdReport;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            logger.LogError("Erro ao criar o relatório: {Error}", error);
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

    public async Task<PagedResponse<List<Report>>> GetListAsync(PagedRequest request)
    {
        try
        {
            var url = $"/reports/list?page={request.Page}&pageSize={request.PageSize}";


            var response = await _httpClient.GetFromJsonAsync<PagedResponse<List<ListReportReponse>>>(url);

            if (response == null || response.Data == null)
                return new PagedResponse<List<Report>>(new List<Report>(), 0, request.Page, request.PageSize);

            var reports = response.Data.Select(r => new Report(
                r.ReportName,
                r.TypeReport,
                r.ReportDescription,
                r.ReportDate,
                r.UserName,
                r.IsEvent,
                r.Actived,
                r.ApplicationUserId
            )
            {
                Id = r.Id
            }).ToList();

            return new PagedResponse<List<Report>>(reports, response.TotalItems, response.PageNumber, response.PageSize);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Erro de requisição ao buscar os relatórios paginados.");
            throw new ApplicationException("Erro ao buscar os relatórios.", ex);
        }
    }

    public Task<IEnumerable<Report>> GetReportsByTypeAsync(string type)
    {
        throw new NotImplementedException();
    }
}

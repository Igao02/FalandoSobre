using FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;
using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Dto.PagedResponse;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.UseCases.ReportUseCase.Delete;
using FalandoSobreApplication.UseCases.ReportUseCase.Edit;
using FalandoSobreApplication.UseCases.ReportUseCase.ListFeed;
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

    public async Task DeleteAsync(Guid id)
    {
        var url = $"/reports/{id}";
        var response = await _httpClient.DeleteAsync(url);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Publicação {ReportId} desativada com sucesso", id);
            return;
        }

        var error = await response.Content.ReadAsStringAsync();
        logger.LogError("Erro ao desativar publicação {ReportId}: {Error}", id, error);
        throw new ApplicationException($"Erro ao desativar publicação: {error}");
    }

    public async Task<Report> EditAsync(Report report)
    {
        var request = new EditReportCommand(
            report.Id,
            report.ReportName,
            report.TypeReport,
            report.ReportDescription
        );

        var response = await _httpClient.PutAsJsonAsync("/reports/edit", request);

        if (response.IsSuccessStatusCode)
        {
            var editedResponse = await response.Content.ReadFromJsonAsync<EditReportResponse>();
            if (editedResponse is null)
            {
                throw new ApplicationException("Resposta inválida ao editar publicação.");
            }

            var editedReport = new Report(
                editedResponse.ReportName,
                editedResponse.TypeReport,
                editedResponse.ReportDescription,
                editedResponse.ReportDate,
                editedResponse.UserName,
                editedResponse.IsEvent,
                editedResponse.Actived,
                editedResponse.ApplicationUserId
            )
            {
                Id = editedResponse.Id
            };

            logger.LogInformation("Publicação atualizada com sucesso: {ReportId}", editedReport.Id);
            return editedReport;
        }

        var error = await response.Content.ReadAsStringAsync();
        logger.LogError("Erro ao editar publicação: {Error}", error);
        throw new ApplicationException($"Erro ao editar publicação: {error}");
    }

    public async Task<List<Report>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient
                .GetFromJsonAsync<List<ListFeedResponse>>("/reports/all");

            if (response == null || !response.Any())
                return new List<Report>();

            var reports = response.Select(r => new Report(
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

            return reports;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Erro ao buscar todos os reports (feed).");
            throw new ApplicationException("Erro ao buscar publicações do feed.", ex);
        }
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

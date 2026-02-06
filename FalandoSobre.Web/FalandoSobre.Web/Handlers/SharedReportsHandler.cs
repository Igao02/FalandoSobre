using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.UseCases.SharedReportUseCase.Create;
using FalandoSobreApplication.UseCases.SharedReportUseCase.List;

namespace FalandoSobre.Web.Handlers;

public class SharedReportsHandler(IHttpClientFactory httpClientFactory, ILogger<SharedReportsHandler> logger)
    : ISharedReportRepository
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    public async Task<SharedReport> Create(SharedReport sharedReport)
    {
        var response = await _httpClient.PostAsJsonAsync("/create-shared-report", new
        {
            sharedReport.ReportId,
            sharedReport.ApplicationUserId,
            sharedReport.Actived,
            sharedReport.CreatedAt,
            sharedReport.UserName,
        });

        if(response.IsSuccessStatusCode)
        {
            var createdReponse = await response.Content.ReadFromJsonAsync<SharedReportResponse>();
            if(createdReponse is null)
            {
                throw new ApplicationException("Resposta inválida ao criar relatório compartilhado.");
            }

            var createdSharedReport = new SharedReport
            {
                Id = createdReponse.Id,
                ReportId = createdReponse.ReportId,
                ApplicationUserId = createdReponse.ApplicationUserId,
                Actived = createdReponse.Actived,
                CreatedAt = createdReponse.CreatedAt,
                UserName = createdReponse.UserName!,
            };
            logger.LogInformation("Relatório compartilhado criado com sucesso!");
            return createdSharedReport;
        }

        var error = await response.Content.ReadAsStringAsync();
        logger.LogError("Erro ao criar relatório compartilhado: {Error}", error);
        throw new ApplicationException($"Erro ao criar relatório compartilhado: {error}");
    }

    public async Task<IEnumerable<SharedReport>> GetListAsync()
    {
        var response = await _httpClient.GetAsync("/list-shared-reports");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            logger.LogError("Erro ao buscar compartilhamentos: {Error}", error);
            throw new ApplicationException($"Erro ao buscar compartilhamentos: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<List<ListSharedReportResponse>>();

        if (result is null)
            return Enumerable.Empty<SharedReport>();

        return result.Select(sr => new SharedReport
        {
            Id = sr.Id,
            ReportId = sr.ReportId,
            ApplicationUserId = sr.ApplicationUserId,
            Actived = sr.Actived,
            CreatedAt = sr.CreatedAt,
            UserName = sr.UserName!,
        });
    }
}

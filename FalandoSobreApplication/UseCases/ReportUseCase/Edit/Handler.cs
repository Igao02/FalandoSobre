using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.ReportUseCase.Edit;

public sealed class EditReportHandler : ICommandHandler<EditReportCommand, EditReportResponse>
{
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<EditReportHandler> _logger;
    private readonly ILogRepository _logRepository;


    public EditReportHandler(IReportRepository reportRepository, ILogger<EditReportHandler> logger, ILogRepository logRepository)
    {
        _reportRepository = reportRepository;
        _logger = logger;
        _logRepository = logRepository;

    }

    public async Task<Result<EditReportResponse>> Handle(EditReportCommand request, CancellationToken cancellationToken)
    {
        if (request.ReportId == Guid.Empty)
        {
            var error = new Error("400", "ReportId não pode ser vazio", ErrorType.Validation);
            _logger.LogWarning("EditReportCommand falhou na validação: {Error}", error);
            return Result.Failure<EditReportResponse>(error);
        }

        try
        {
            var existing = await _reportRepository.GetAsync(request.ReportId);
            if (existing is null)
            {
                var error = new Error("404", "Publicação não encontrada", ErrorType.NotFound);
                return Result.Failure<EditReportResponse>(error);
            }

            existing.ReportName = request.ReportName;
            existing.TypeReport = request.TypeReport;
            existing.ReportDescription = request.ReportDescription;

            var updated = await _reportRepository.EditAsync(existing);

            var response = new EditReportResponse
            {
                Id = updated.Id,
                ReportName = updated.ReportName,
                TypeReport = updated.TypeReport,
                ReportDescription = updated.ReportDescription,
                ReportDate = updated.ReportsDate,
                UserName = updated.UserName!,
                IsEvent = updated.IsEvent ?? false,
                ApplicationUserId = updated.ApplicationUserId,
                Actived = updated.Actived
            };

            _logger.LogInformation("Publicação atualizada com sucesso. ReportId: {ReportId}", updated.Id);

            var log = new Logs
            {
                Action = "Publicação editada com sucesso",
                ApplicationUserId = existing.ApplicationUserId,
                Created_At = DateTime.UtcNow,
                EntityType = "Report",
                UserName = existing.UserName,
            };
            await _logRepository.Create(log);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar publicação ReportId: {ReportId}", request.ReportId);
            return Result.Failure<EditReportResponse>(new Error("500", "Erro interno do servidor", ErrorType.Failure));
        }
    }
}

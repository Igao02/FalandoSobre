using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.ReportUseCase.Delete;

public sealed class DeleteReportHandler : ICommandHandler<DeleteReportCommand, DeleteReportResponse>
{
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<DeleteReportHandler> _logger;
    private readonly ILogRepository _logRepository;

    public DeleteReportHandler(IReportRepository reportRepository, ILogger<DeleteReportHandler> logger , ILogRepository logRepository)
    {
        _reportRepository = reportRepository;
        _logger = logger;
        _logRepository = logRepository;

    }

    public async Task<Result<DeleteReportResponse>> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
    {
        if (request.ReportId == Guid.Empty)
        {
            var error = new Error("400", "ReportId não pode ser vazio", ErrorType.Validation);
            _logger.LogWarning("DeleteReportCommand falhou na validação: {Error}", error);
            return Result.Failure<DeleteReportResponse>(error);
        }

        try
        {
            var existing = await _reportRepository.GetAsync(request.ReportId);
            if (existing is null)
            {
                var error = new Error("404", "Publicação não encontrada", ErrorType.NotFound);
                return Result.Failure<DeleteReportResponse>(error);
            }

            await _reportRepository.DeleteAsync(request.ReportId);

            var response = new DeleteReportResponse
            {
                Success = true,
                Message = "Publicação desativada com sucesso"
            };

            _logger.LogInformation("Publicação desativada com sucesso. ReportId: {ReportId}", request.ReportId);

            var log = new Logs
            {
                Action = "Publicação excluída com sucesso",
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
            _logger.LogError(ex, "Erro ao desativar publicação ReportId: {ReportId}", request.ReportId);
            return Result.Failure<DeleteReportResponse>(new Error("500", "Erro interno do servidor", ErrorType.Failure));
        }
    }
}

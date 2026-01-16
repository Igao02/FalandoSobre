using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.SharedReportUseCase.Create;

public sealed class CreateSharedReportHandler : ICommandHandler<CreateSharedReportCommand, SharedReportResponse>
{
    private readonly ISharedReportRepository _sharedReportRepository;
    private readonly ILogRepository _logRepository;
    private readonly ILogger<CreateSharedReportHandler> _logger;

    public CreateSharedReportHandler(ISharedReportRepository sharedReportRepository, ILogRepository logRepository, ILogger<CreateSharedReportHandler> logger)
    {
        _sharedReportRepository = sharedReportRepository;
        _logRepository = logRepository;
        _logger = logger;
    }

    public async Task<Result<SharedReportResponse>> Handle(CreateSharedReportCommand request, CancellationToken cancellationToken)
    {
        if (request.ReportId == Guid.Empty)
        {
            var error = new Error("400", "The ReportId provided is invalid.", ErrorType.Validation);
            _logger.LogWarning("CreateSharedReportHandler Validation failed: {Error}", error);
            return Result<SharedReportResponse>.ValidationFailure(error);
        }

        try
        {
            var sharedReport = new SharedReport
            {
                Actived = true,
                ApplicationUserId = request.ApplicationUserId,
                CreatedAt = DateTime.UtcNow,
                ReportId = request.ReportId
            };

            var createdSharedReport = await _sharedReportRepository.Create(sharedReport);

            var response = new SharedReportResponse
            {
                Id = createdSharedReport.Id,
                ReportId = createdSharedReport.ReportId,
                ApplicationUserId = createdSharedReport.ApplicationUserId,
                CreatedAt = createdSharedReport.CreatedAt,
                Actived = createdSharedReport.Actived
            };

            var log = new Logs
            {
                Action = "Publicação Compartilhada",
                EntityType = "SharedReport",
                Created_At = DateTime.UtcNow,
                ApplicationUserId = request.ApplicationUserId,
            };

            await _logRepository.Create(log);
            _logger.LogInformation("Compartilhamento criado com sucesso para a publicação: {ReportId}", request.ReportId);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar compartilhamento para a publicação: {ReportId}", request.ReportId);
            return Result.Failure<SharedReportResponse>(new Error("500", "Um erro ocorreu ao compartilhar a publicação.", ErrorType.Failure));
        }
    }
}

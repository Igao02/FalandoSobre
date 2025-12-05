using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;

public sealed class CreateReportHandler : ICommandHandler<CreateReportCommand, CreateReportResponse>
{
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<CreateReportHandler> _logger;
    private readonly ILogRepository _logRepository;
    private readonly IInstitutionRepository? _institutionRepository;

    public CreateReportHandler(IReportRepository reportRepository, ILogger<CreateReportHandler> logger, ILogRepository logRepository, IInstitutionRepository? institutionRepository)
    {
        _reportRepository = reportRepository;
        _logger = logger;
        _logRepository = logRepository;
        _institutionRepository = institutionRepository;
    }

    public async Task<Result<CreateReportResponse>> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            var error = new Error("401", "Usuário não autenticado", ErrorType.Unauthorized);
            _logger.LogInformation("Usuário não autenticado: {Error}", error);
            return Result.Failure<CreateReportResponse>(error);
        }

        try
        {
            var institution = _institutionRepository is null
                ? null
                : await _institutionRepository.GetByApplicationUserIdAsync(request.ApplicationUserId);

            if (institution == null)
            {
                _logger.LogInformation("Nenhuma instituição encontrada para o usuário {UserId}", request.ApplicationUserId);
            }

            var report = new Report(
                request.ReportName,
                request.TypeReport,
                request.ReportDescription,
                DateTime.UtcNow,
                request.UserName,
                institution?.ApplicationUserId == request.ApplicationUserId ? true : false,
                request.Actived,
                request.ApplicationUserId
            );

            var createdReport = await _reportRepository.AddAsync(report);
            _logger.LogInformation("Relatório criado com ID: {ReportId}", createdReport.Id);

            var response = new CreateReportResponse
            {
                Id = createdReport.Id,
                ReportName = createdReport.ReportName,
                TypeReport = createdReport.TypeReport,
                ReportDescription = createdReport.ReportDescription,
                ReportDate = createdReport.ReportsDate,
                UserName = createdReport.UserName!,
                IsEvent = createdReport.IsEvent ?? false,
                Actived = createdReport.Actived,
                ApplicationUserId = createdReport.ApplicationUserId
            };

            var log = new Logs
            {
                Action = "Publicação criada com sucesso",
                ApplicationUserId = request.ApplicationUserId,
                Created_At = DateTime.UtcNow,
                EntityType = "Report",
                UserName = request.UserName,
            };

            await _logRepository.Create(log);

            return Result.Success(response);
        }
        catch
        {
            var error = new Error("500", "Erro ao criar o relatório", ErrorType.Failure);
            _logger.LogInformation("Erro ao criar o relatório: {Error}", error);
            return Result.Failure<CreateReportResponse>(error);
        }
    }
}


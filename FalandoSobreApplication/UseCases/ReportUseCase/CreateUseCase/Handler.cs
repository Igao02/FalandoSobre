using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;

public sealed class CreateReportHandler : ICommandHandler<CreateReportCommand, Report>
{
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<CreateReportHandler> _logger;

    public CreateReportHandler(IReportRepository reportRepository, ILogger<CreateReportHandler> logger)
    {
        _reportRepository = reportRepository;
        _logger = logger;
    }

    public async Task<Result<Report>> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            var error = new Error("401", "Usuário não autenticado", ErrorType.Unauthorized);
            _logger.LogInformation("Usuário não autenticado: {Error}", error);
            return Result.Failure<Report>(error);
        }

        _logger.LogInformation("Iniciando criação do relatório para o usuário {UserName}", request);

        try
        {
            var report = new Report(
                request.ReportName,
                request.TypeReport,
                request.ReportDescription,
                DateTime.UtcNow,
                request.UserName,
                request.IsEvent
            );

            _logger.LogInformation("CRIADO O RELATÓRIO COM SUCESSO: {Report}", report.Id);
            var createdReport = await _reportRepository.AddAsync(report);
            _logger.LogInformation("ID RETORNADO NESSA MERDA: {Report}", createdReport.Id);
            return Result.Success(createdReport);
        }
        catch
        {
            var error = new Error("500", "Erro ao criar o relatório", 0);
            _logger.LogInformation("Erro ao criar o relatório: {Error}", error);
            return Result.Failure<Report>(error);
        }
    }
}

using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;

public sealed class CreateReportHandler : ICommandHandler<CreateReportCommand, Guid>
{
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<CreateReportHandler> _logger;

    public CreateReportHandler(IReportRepository reportRepository, ILogger<CreateReportHandler> logger)
    {
        _reportRepository = reportRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateReportCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UserName))
        {
            var error = new Error("401", "Usuário não autenticado", ErrorType.Unauthorized);
            _logger.LogInformation("Usuário não autenticado: {Error}", error);
            return Result.Failure<Guid>(error);
        }

        try
        {
            var report = new Report(
                command.ReportName,
                command.TypeReport,
                command.ReportDescription,
                DateTime.UtcNow,
                command.UserName,
                command.IsEvent
            );

            var createdReport = await _reportRepository.AddAsync(report);
            _logger.LogInformation("CRIADO O RELATÓRIO COM SUCESSO: {Report}", report);
            return Result.Success(report.Id);
        }
        catch
        {
            var error = new Error("500", "Erro ao criar o relatório", 0);
            _logger.LogInformation("Erro ao criar o relatório: {Error}", error);
            return Result.Failure<Guid>(error);
        }
    }
}

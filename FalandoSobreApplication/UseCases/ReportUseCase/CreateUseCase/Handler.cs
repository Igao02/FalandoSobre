using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;

public sealed class CreateReportHandler : ICommandHandler<CreateReportCommand, Guid>
{
    private readonly IReportRepository _reportRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CreateReportHandler> _logger;

    public CreateReportHandler(IReportRepository reportRepository, IHttpContextAccessor httpContextAccessor, ILogger<CreateReportHandler> logger)
    {
        _reportRepository = reportRepository;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateReportCommand command, CancellationToken cancellationToken)
    {
        //var user = _httpContextAccessor.HttpContext?.User;
        //_logger.LogInformation("User: {User}", user);

        if (command.UserName == null)
        {
            throw new InvalidOperationException("Usuário não autenticado");
        }

        //var userName = user?.Identity?.Name;

        var report = new Report(
            command.ReportName,
            command.TypeReport,
            command.ReportDescription,
            DateTime.UtcNow,
            command.UserName,
            command.IsEvent
        );
        _logger.LogInformation("CRIANDO O RELATÓRIO: {Report}", report);

        var createdReport = await _reportRepository.AddAsync(report);
        _logger.LogInformation("CRIADO O RELATÓRIO COM SUCESSO: {Report}", report);

        return Result.Success(report.Id);
    }
}

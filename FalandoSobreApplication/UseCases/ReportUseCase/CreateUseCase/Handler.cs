using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
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
        var user = _httpContextAccessor.HttpContext?.User;
        _logger.LogInformation("CreateReportHandler Handle user: {User}", user?.Identity?.Name);
        

        if (user is null || !user.Identity?.IsAuthenticated == true)
        {
            throw new InvalidOperationException("Usuário não autenticado");
        }

        var userName = user?.Identity?.Name;
        _logger.LogInformation("CreateReportHandler Handle userName: {UserName}", userName);
  

        var report = new Report(
            command.ReportName,
            command.TypeReport,
            command.ReportDescription,
            DateTime.UtcNow,
            userName,
            command.IsEvent
        );

        await _reportRepository.AddAsync(report);

        return Result.Success(report.Id);
    }
}

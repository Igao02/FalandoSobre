using Application.Abstractions.Messaging;

namespace FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;

public sealed record CreateReportCommand(string ReportName, string TypeReport, string ReportDescription,  bool IsEvent)
    : ICommand<Guid>;

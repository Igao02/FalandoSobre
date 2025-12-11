using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.ReportUseCase.Delete;

public sealed record DeleteReportCommand(
    Guid ReportId
) : ICommand<DeleteReportResponse>;

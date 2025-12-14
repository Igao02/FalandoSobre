using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.ReportUseCase.Edit;

public sealed record EditReportCommand(
    Guid ReportId,
    string ReportName,
    string TypeReport,
    string ReportDescription
) : ICommand<EditReportResponse>;

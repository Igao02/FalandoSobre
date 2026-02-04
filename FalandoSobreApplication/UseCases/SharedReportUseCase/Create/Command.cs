using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.SharedReportUseCase.Create;

public sealed record CreateSharedReportCommand(
    Guid ReportId,
    string ApplicationUserId,
    string UserName
) : ICommand<SharedReportResponse>;

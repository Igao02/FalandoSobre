using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.LikeUseCase.ListByReportId;

public sealed record ListLikesByReportIdCommand(
    Guid ReportId
) : ICommand<ListLikesByReportIdResponse>;

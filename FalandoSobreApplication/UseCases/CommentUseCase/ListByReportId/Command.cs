using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.CommentUseCase.ListByReportId;

public sealed record ListCommentsByReportIdCommand(
    Guid ReportId
) : ICommand<ListCommentsByReportIdResponse>;

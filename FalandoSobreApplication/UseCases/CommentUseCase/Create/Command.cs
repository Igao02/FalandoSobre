using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.CommentUseCase.Create;

public sealed record CreateCommentCommand(
    Guid ReportId,
    string CommentContent,
    string UserName,
    string ApplicationUserId
) : ICommand<CommentResponse>;

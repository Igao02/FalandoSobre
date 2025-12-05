using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.CommentUseCase.Delete;

public sealed record DeleteCommentCommand(
    Guid CommentId
) : ICommand<DeleteCommentResponse>;

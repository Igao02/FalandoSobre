namespace FalandoSobreApplication.UseCases.CommentUseCase.Delete;

public sealed class DeleteCommentResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

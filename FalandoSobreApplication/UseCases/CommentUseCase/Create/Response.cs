namespace FalandoSobreApplication.UseCases.CommentUseCase.Create;

public sealed class CommentResponse
{
    public Guid Id { get; init; }
    public Guid ReportId { get; init; }
    public string CommentContent { get; init; } = string.Empty;
    public DateTime CommentDate { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? ApplicationUserId { get; init; }
}

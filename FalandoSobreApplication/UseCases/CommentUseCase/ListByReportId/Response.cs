namespace FalandoSobreApplication.UseCases.CommentUseCase.ListByReportId;

public sealed class ListCommentsByReportIdResponse
{
    public int TotalComments { get; init; }
    public List<CommentDto> Comments { get; init; } = new();
}

public sealed class CommentDto
{
    public Guid Id { get; init; }
    public Guid ReportId { get; init; }
    public string CommentContent { get; init; } = string.Empty;
    public DateTime CommentDate { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? ApplicationUserId { get; init; }
}

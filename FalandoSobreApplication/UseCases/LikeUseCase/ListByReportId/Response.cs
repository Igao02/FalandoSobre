namespace FalandoSobreApplication.UseCases.LikeUseCase.ListByReportId;

public sealed class ListLikesByReportIdResponse
{
    public int TotalLikes { get; init; }
    public List<LikeDto> Likes { get; init; } = new();
}

public sealed class LikeDto
{
    public Guid Id { get; init; }
    public string? ApplicationUserId { get; init; }
    public DateTime LikeDate { get; init; }
}

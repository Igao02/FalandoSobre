namespace FalandoSobreApplication.UseCases.LikeUseCase.ListByUserId;

public sealed class ListLikesByUserIdResponse
{
    public List<LikeDto> Likes { get; init; } = new();
}

public sealed class LikeDto
{
    public Guid Id { get; init; }
    public Guid ReportId { get; init; }
    public string? ApplicationUserId { get; init; }
    public bool Actived { get; init; }
    public DateTime LikeDate { get; init; }
}

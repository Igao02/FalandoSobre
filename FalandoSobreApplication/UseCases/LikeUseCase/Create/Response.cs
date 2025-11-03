namespace FalandoSobreApplication.UseCases.LikeUseCase.Create;

public sealed class LikeResponse
{
    public Guid Id { get; init; }
    public bool Actived { get; init; }
    public string? ApplicationUserId { get; init; }
}

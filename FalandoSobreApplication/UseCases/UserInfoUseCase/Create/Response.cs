namespace FalandoSobreApplication.UseCases.UserInfoUseCase.Create;

public sealed class CreateUserInfoResponse
{
    public Guid Id { get; init; }
    public string ProfilePhoto { get; init; } = default!;
    public DateTime CreatedAt { get; init; }
    public bool Actived { get; init; } = default!;
    public string ApplicationUserId { get; init; } = default!;
}

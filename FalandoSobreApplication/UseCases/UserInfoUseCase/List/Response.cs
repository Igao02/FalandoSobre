namespace FalandoSobreApplication.UseCases.UserInfoUseCase.List;

public class ListUserInfoResponse
{
    public Guid Id { get; init; }
    public string? ProfilePhoto { get; init; }
    public string? ApplicationUserId { get; init; }
    public bool Actived { get; init; }
    public DateTime CreatedAt { get; init; }
}

namespace FalandoSobreApplication.UseCases.UserInfoUseCase.GetByUseId;

public sealed class GetByUserIdResponse
{
    public Guid Id { get; init; }
    
    public string? ProfilePhoto { get; init; }
    
    public string? ApplicationUserId { get; init; }

}

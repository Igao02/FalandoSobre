using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.UserInfoUseCase.Create;

public sealed record CreateUserInfoCommand(
    string ProfilePhoto,
    string ApplicationUserId,
    byte[] ProfilePhotoBytes
) : ICommand<CreateUserInfoResponse>;


using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.UserInfoUseCase.GetByUseId;

public sealed record GetByUserIdCommand(string ApplicationUserId) : ICommand<GetByUserIdResponse>;


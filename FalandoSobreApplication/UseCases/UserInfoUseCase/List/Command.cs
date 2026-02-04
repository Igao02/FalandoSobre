using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.UserInfoUseCase.List;

public sealed record ListUserInfoCommand() : ICommand<List<ListUserInfoResponse>>;


using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.LikeUseCase.ListByUserId;

public sealed record ListLikesByUserIdCommand(
    string UserId
) : ICommand<ListLikesByUserIdResponse>;

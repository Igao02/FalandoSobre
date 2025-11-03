using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.LikeUseCase.Delete;

public sealed record DeleteLikeCommand(
    string UserId,
    Guid ReportId
) : ICommand<DeleteLikeResponse>;

using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.LikeUseCase.Create;

public sealed record CreateLikeCommand(
    Guid ReportId,
    string? ApplicationUserId
) : ICommand<LikeResponse>;


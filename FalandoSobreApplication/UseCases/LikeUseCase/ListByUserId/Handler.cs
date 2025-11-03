using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.LikeUseCase.ListByUserId;

public sealed class ListLikesByUserIdHandler : ICommandHandler<ListLikesByUserIdCommand, ListLikesByUserIdResponse>
{
    private readonly ILikeRepository _likeRepository;
    private readonly ILogger<ListLikesByUserIdHandler> _logger;

    public ListLikesByUserIdHandler(ILikeRepository likeRepository, ILogger<ListLikesByUserIdHandler> logger)
    {
        _likeRepository = likeRepository;
        _logger = logger;
    }

    public async Task<Result<ListLikesByUserIdResponse>> Handle(ListLikesByUserIdCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UserId))
        {
            var error = new Error("400", "UserId não pode ser vazio", ErrorType.Validation);
            _logger.LogWarning("ListLikesByUserIdQuery failed validation: {Error}", error);
            return Result.Failure<ListLikesByUserIdResponse>(error);
        }

        try
        {
            var likes = await _likeRepository.GetLikesByUserIdAsync(request.UserId);

            var response = new ListLikesByUserIdResponse
            {
                Likes = likes.Select(like => new LikeDto
                {
                    Id = like.Id,
                    ReportId = like.ReportId,
                    ApplicationUserId = like.ApplicationUserId,
                    Actived = like.Actived,
                    LikeDate = new DateTime()
                }).ToList()
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar likes do usuário com ID: {UserId}", request.UserId);
            return Result.Failure<ListLikesByUserIdResponse>(new Error("500", "Erro interno do servidor", ErrorType.Failure));
        }
    }
}

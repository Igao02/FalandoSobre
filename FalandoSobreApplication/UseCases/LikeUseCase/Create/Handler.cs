using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.LikeUseCase.Create;

public sealed class CreateLikeHandler : ICommandHandler<CreateLikeCommand, LikeResponse>
{
    private readonly ILikeRepository _likeRepository;
    private readonly ILogger<CreateLikeHandler> _logger;
    private readonly ILogRepository _logRepository;

    public CreateLikeHandler(ILikeRepository likeRepository, ILogger<CreateLikeHandler> logger, ILogRepository logRepository)
    {
        _likeRepository = likeRepository;
        _logger = logger;
        _logRepository = logRepository;
    }

    public async Task<Result<LikeResponse>> Handle(CreateLikeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.ApplicationUserId))
        {
            var error = new Error("401", "Usuário não autenticado", ErrorType.Unauthorized);
            _logger.LogWarning("CreateLikeCommand failed validation: {Error}", error);
            return Result.Failure<LikeResponse>(error);
        }

        try
        {
            var like = new Like
            {
                Actived = true,
                ApplicationUserId = request.ApplicationUserId,
                LikeDate = DateTime.UtcNow,
                ReportId = request.ReportId,
            };

            var createdLike = await _likeRepository.AddLikesAsync(like);

            var response = new LikeResponse
            {
                Id = createdLike.Id,
                Actived = createdLike.Actived,
                ApplicationUserId = createdLike.ApplicationUserId,
            };

            var log = new Logs
            {
                Action = "Curtida adicionada com sucesso!",
                Created_At = DateTime.UtcNow,
                EntityType = "Like",
                ApplicationUserId = request.ApplicationUserId,
            };

            await _logRepository.Create(log);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar o like, usuário com ID: {ApplicationUserId}", request.ApplicationUserId);
            return Result.Failure<LikeResponse>(new Error("500", "Erro interno do servidor", ErrorType.Failure));
        }
    }
}

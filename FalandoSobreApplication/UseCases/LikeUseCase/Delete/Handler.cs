using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.LikeUseCase.Delete;

public sealed class DeleteLikeHandler : ICommandHandler<DeleteLikeCommand, DeleteLikeResponse>
{
    private readonly ILikeRepository _likeRepository;
    private readonly ILogger<DeleteLikeHandler> _logger;
    private readonly ILogRepository _logRepository;

    public DeleteLikeHandler(ILikeRepository likeRepository, ILogger<DeleteLikeHandler> logger, ILogRepository logRepository)
    {
        _likeRepository = likeRepository;
        _logger = logger;
        _logRepository = logRepository;
    }

    public async Task<Result<DeleteLikeResponse>> Handle(DeleteLikeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UserId))
        {
            var error = new Error("401", "Usuário não autenticado", ErrorType.Unauthorized);
            _logger.LogWarning("DeleteLikeCommand failed validation: {Error}", error);
            return Result.Failure<DeleteLikeResponse>(error);
        }

        try
        {
            var existingLike = await _likeRepository.GetLikeByUserAndReportAsync(request.UserId, request.ReportId);

            if (existingLike == null)
            {
                var error = new Error("404", "Like não encontrado", ErrorType.NotFound);
                return Result.Failure<DeleteLikeResponse>(error);
            }

            await _likeRepository.RemoveLikeAsync(existingLike.ApplicationUserId!, request.ReportId);

            var log = new Logs
            {
                Action = "Curtida removida com sucesso!",
                Created_At = DateTime.UtcNow,
                EntityType = "Like",
                ApplicationUserId = request.UserId,
            };

            await _logRepository.Create(log);

            var response = new DeleteLikeResponse
            {
                Success = true,
                Message = "Like removido com sucesso"
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover o like, usuário com ID: {UserId}, Report ID: {ReportId}", request.UserId, request.ReportId);
            return Result.Failure<DeleteLikeResponse>(new Error("500", "Erro interno do servidor", ErrorType.Failure));
        }
    }
}

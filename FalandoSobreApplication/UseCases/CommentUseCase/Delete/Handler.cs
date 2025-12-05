using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.CommentUseCase.Delete;

public sealed class DeleteCommentHandler : ICommandHandler<DeleteCommentCommand, DeleteCommentResponse>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<DeleteCommentHandler> _logger;

    public DeleteCommentHandler(ICommentRepository commentRepository, ILogger<DeleteCommentHandler> logger)
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<Result<DeleteCommentResponse>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        if (request.CommentId == Guid.Empty)
        {
            var error = new Error("400", "CommentId não pode ser vazio", ErrorType.Validation);
            _logger.LogWarning("DeleteCommentCommand failed validation: {Error}", error);
            return Result.Failure<DeleteCommentResponse>(error);
        }

        try
        {
            var existing = await _commentRepository.GetAsync(request.CommentId);
            if (existing is null)
            {
                var error = new Error("404", "Comentário não encontrado", ErrorType.NotFound);
                return Result.Failure<DeleteCommentResponse>(error);
            }

            await _commentRepository.DeleteAsync(request.CommentId);

            var response = new DeleteCommentResponse
            {
                Success = true,
                Message = "Comentário removido com sucesso"
            };

            _logger.LogInformation("Comentário removido com sucesso. CommentId: {CommentId}", request.CommentId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover comentário CommentId: {CommentId}", request.CommentId);
            return Result.Failure<DeleteCommentResponse>(new Error("500", "Erro interno do servidor", ErrorType.Failure));
        }
    }
}

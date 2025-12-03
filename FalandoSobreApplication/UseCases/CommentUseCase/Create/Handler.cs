using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.CommentUseCase.Create;

public sealed class CreateCommentHandler : ICommandHandler<CreateCommentCommand, CommentResponse>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogRepository _logRepository;
    private readonly ILogger<CreateCommentHandler> _logger;

    public CreateCommentHandler(ICommentRepository commentRepository, ILogRepository logRepository, ILogger<CreateCommentHandler> logger)
    {
        _commentRepository = commentRepository;
        _logRepository = logRepository;
        _logger = logger;
    }

    public async Task<Result<CommentResponse>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        if (request.ReportId == Guid.Empty)
        {
            var error = new Error("400", "ReportId não pode ser vazio", ErrorType.Validation);
            _logger.LogInformation("CreateCommentCommand failed validation: {Error}", error);
            return Result.Failure<CommentResponse>(error);
        }

        if (string.IsNullOrWhiteSpace(request.CommentContent))
        {
            var error = new Error("400", "Conteúdo do comentário é obrigatório", ErrorType.Validation);
            _logger.LogInformation("CreateCommentCommand failed validation: {Error}", error);
            return Result.Failure<CommentResponse>(error);
        }

        try
        {
            var comment = new Comment{
                Actived = true,
                CommentContent = request.CommentContent,
                CommentDate = DateTime.UtcNow,
                ReportId = request.ReportId,
                UserName = request.UserName,
                ApplicationUserId = request.ApplicationUserId
            };

            var createdComment = await _commentRepository.AddAsync(comment);

            var response = new CommentResponse
            {
                Id = createdComment.Id,
                ReportId = createdComment.ReportId,
                CommentContent = createdComment.CommentContent,
                CommentDate = createdComment.CommentDate,
                UserName = createdComment.UserName,
                ApplicationUserId = createdComment.ApplicationUserId
            };

            var log = new Logs
            {
                UserName = request.UserName,
                Action = "Comentário criado com sucesso!",
                EntityType = "Comment",
                Created_At = DateTime.UtcNow,
                ApplicationUserId = request.ApplicationUserId
            };

            await _logRepository.Create(log);
            _logger.LogInformation("Comentário criado com sucesso para o report {ReportId}", request.ReportId);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar comentário para o report {ReportId}", request.ReportId);
            return Result.Failure<CommentResponse>(new Error("500", "Erro interno do servidor", ErrorType.Failure));
        }
    }
}

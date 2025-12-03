using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FalandoSobreApplication.UseCases.CommentUseCase.ListByReportId;

public sealed class ListCommentsByReportIdHandler : ICommandHandler<ListCommentsByReportIdCommand, ListCommentsByReportIdResponse>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<ListCommentsByReportIdHandler> _logger;

    public ListCommentsByReportIdHandler(ICommentRepository commentRepository, ILogger<ListCommentsByReportIdHandler> logger)
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<Result<ListCommentsByReportIdResponse>> Handle(ListCommentsByReportIdCommand request, CancellationToken cancellationToken)
    {
        if (request.ReportId == Guid.Empty)
        {
            var error = new Error("400", "ReportId não pode ser vazio", ErrorType.Validation);
            _logger.LogWarning("ListCommentsByReportIdCommand failed validation: {Error}", error);
            return Result.Failure<ListCommentsByReportIdResponse>(error);
        }

        try
        {
            var commentsForReport = (await _commentRepository.GetByReportIdAsync(request.ReportId)).ToList();

            var response = new ListCommentsByReportIdResponse
            {
                TotalComments = commentsForReport.Count,
                Comments = commentsForReport.Select(c => new CommentDto
                {
                    Id = c.Id,
                    ReportId = c.ReportId,
                    CommentContent = c.CommentContent,
                    CommentDate = c.CommentDate,
                    UserName = c.UserName,
                    ApplicationUserId = c.ApplicationUserId
                }).ToList()
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar comentários do report {ReportId}", request.ReportId);
            return Result.Failure<ListCommentsByReportIdResponse>(new Error("500", "Erro interno do servidor", ErrorType.Failure));
        }
    }
}

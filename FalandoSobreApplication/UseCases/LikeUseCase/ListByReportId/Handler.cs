using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.LikeUseCase.ListByReportId;

public sealed class ListLikesByReportIdHandler : ICommandHandler<ListLikesByReportIdCommand, ListLikesByReportIdResponse>
{
    private readonly ILikeRepository _likeRepository;
    private readonly ILogger<ListLikesByReportIdHandler> _logger;

    public ListLikesByReportIdHandler(ILikeRepository likeRepository, ILogger<ListLikesByReportIdHandler> logger)
    {
        _likeRepository = likeRepository;
        _logger = logger;
    }

    public async Task<Result<ListLikesByReportIdResponse>> Handle(ListLikesByReportIdCommand request, CancellationToken cancellationToken)
    {
        if (request.ReportId == Guid.Empty)
        {
            var error = new Error("400", "ReportId não pode ser vazio", ErrorType.Validation);
            _logger.LogWarning("ListLikesByReportIdCommand failed validation: {Error}", error);
            return Result.Failure<ListLikesByReportIdResponse>(error);
        }

        try
        {
            var likes = await _likeRepository.GetLikesByReportIdAsync(request.ReportId);

            var response = new ListLikesByReportIdResponse
            {
                TotalLikes = likes.Count(),
                Likes = likes.Select(like => new LikeDto
                {
                    Id = like.Id,
                    ApplicationUserId = like.ApplicationUserId,
                    LikeDate = (DateTime)like.LikeDate!
                }).ToList()
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar likes do report com ID: {ReportId}", request.ReportId);
            return Result.Failure<ListLikesByReportIdResponse>(new Error("500", "Erro interno do servidor", ErrorType.Failure));
        }
    }
}

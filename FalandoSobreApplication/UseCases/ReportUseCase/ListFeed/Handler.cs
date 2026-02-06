using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.ReportUseCase.ListFeed
{
    public class ListFeedHandler : ICommandHandler<ListFeedCommand, List<ListFeedResponse>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly ILogger<ListFeedHandler> _logger;
        public ListFeedHandler(IReportRepository reportRepository, ILogger<ListFeedHandler> logger)
        {
            _reportRepository = reportRepository;
            _logger = logger;
        }

        public async Task<Result<List<ListFeedResponse>>> Handle(ListFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var reports = _reportRepository.GetAllAsync();

                var response = reports.Result.Select(report => new ListFeedResponse
                {
                    Id = report.Id,
                    ReportName = report.ReportName,
                    TypeReport = report.TypeReport,
                    ReportDescription = report.ReportDescription,
                    ReportDate = report.ReportsDate,
                    UserName = report.UserName!,
                    IsEvent = report.IsEvent ?? false,
                    ApplicationUserId = report.ApplicationUserId,
                    Actived = report.Actived
                }).ToList();

                return Result.Success(response);
            }
            catch (Exception ex) 
            {
                var error = new Error("500", "Erro ao listar as publicações", ErrorType.Failure);
                _logger.LogError(ex, "Erro ao listar as publicações");
                return Result.Failure<List<ListFeedResponse>>(error);
            }
        }
    }
}

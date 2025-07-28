using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Dto.PagedResponse;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.ReportUseCase.ListUseCase
{
    public sealed class ListReportHandler : ICommandHandler<ListReportCommand, PagedResponse<ListReportReponse[]>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly ILogger<ListReportHandler> _logger;

        public ListReportHandler(IReportRepository reportRepository, ILogger<ListReportHandler> logger)
        {
            _reportRepository = reportRepository;
            _logger = logger;
        }

        public async Task<Result<PagedResponse<ListReportReponse[]>>> Handle(ListReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var pagedResult = await _reportRepository.GetListAsync(new PagedRequest
                {
                    Page = request.Page,
                    PageSize = request.PageSize
                });

                var response = pagedResult.Data.Select(report => new ListReportReponse
                {
                    Id = report.Id,
                    ReportName = report.ReportName,
                    TypeReport = report.TypeReport,
                    ReportDescription = report.ReportDescription,
                    ReportDate = report.ReportsDate,
                    UserName = report.UserName!,
                    IsEvent = report.IsEvent ?? false,
                    ApplicationUserId = report.ApplicationUserId!,
                    Actived = report.Actived
                }).ToArray();

                var finalResponse = new PagedResponse<ListReportReponse[]>(response, pagedResult.TotalItems, request.Page, request.PageSize);

                return Result.Success(finalResponse);
            }
            catch (Exception ex)
            {
                var error = new Error("500", "Erro ao listar relatórios", ErrorType.Failure);
                _logger.LogError(ex, "Erro ao listar relatórios: {Error}", error);
                return Result.Failure<PagedResponse<ListReportReponse[]>>(error);
            }
        }
    }
}

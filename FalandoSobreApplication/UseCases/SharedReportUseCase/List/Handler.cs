using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.SharedReportUseCase.List;

public sealed class ListSharedReportHandler(
    ISharedReportRepository sharedReportRepository,
    ILogger<ListSharedReportHandler> logger)
    : ICommandHandler<ListSharedReportCommand, List<ListSharedReportResponse>>
{
    public async Task<Result<List<ListSharedReportResponse>>> Handle(
        ListSharedReportCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var sharedReports = await sharedReportRepository.GetListAsync();

            var response = sharedReports
                .Select(sharedReport => new ListSharedReportResponse
                {
                    Id = sharedReport.Id,
                    ReportId = sharedReport.ReportId,
                    ApplicationUserId = sharedReport.ApplicationUserId,
                    Actived = sharedReport.Actived,
                    CreatedAt = sharedReport.CreatedAt
                })
                .ToList();

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            var error = new Error(
                "500",
                "Erro ao listar os compartilhamentos",
                ErrorType.NotFound
            );

            logger.LogError(ex, "Erro ao listar os compartilhamentos");
            return Result.Failure<List<ListSharedReportResponse>>(error);
        }
    }
}

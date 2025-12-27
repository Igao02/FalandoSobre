using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.Domain.Dto.PagedResponse;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.ReportUseCase.ListUseCase;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Report;

public sealed class ListReportEndpoint : IEndpoint
{

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/reports/list", async (
        int page,
        int pageSize,
        ISender sender,
        CancellationToken cancellationToken) =>
        {
            var command = new ListReportCommand(page, pageSize);
            Result<PagedResponse<ListReportReponse[]>> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("ListReports")
        .WithTags("Reports");
    }
}


using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.SharedReportUseCase.List;
using MediatR;

namespace FalandoSobre.Api.Endpoints.SharedReport;

public class ListSharedReportEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/list-shared-reports", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new ListSharedReportCommand();
            Result<List<ListSharedReportResponse>> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("ListSharedReports")
        .WithTags("SharedReports");
    }
}

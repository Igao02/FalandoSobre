using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.ReportUseCase.Delete;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Report;

public sealed class DeleteReportEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/reports/{reportId}", async (Guid reportId, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new DeleteReportCommand(reportId);

            Result<DeleteReportResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("DeleteReport")
        .WithTags("Reports");
    }
}

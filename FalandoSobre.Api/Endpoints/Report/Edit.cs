using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.ReportUseCase.Edit;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Report;

public sealed class EditReportEndpoint : IEndpoint
{
    public sealed record Request(Guid ReportId, string ReportName, string TypeReport, string ReportDescription);

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/reports/edit", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new EditReportCommand(
                request.ReportId,
                request.ReportName,
                request.TypeReport,
                request.ReportDescription
            );

            Result<EditReportResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("EditReport")
        .WithTags("Reports")
        .WithOpenApi();
    }
}

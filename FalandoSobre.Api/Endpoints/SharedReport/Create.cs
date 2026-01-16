
using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.SharedReportUseCase.Create;
using MediatR;

namespace FalandoSobre.Api.Endpoints.SharedReport;

public sealed class CreateSharedReportEndpoint : IEndpoint
{
    public sealed record Request(Guid ReportId, string ApplicationUserId);

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/create-shared-report", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateSharedReportCommand(
                request.ReportId,
                request.ApplicationUserId
            );
            Result<SharedReportResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("CreateSharedReport")
        .WithTags("SharedReport");
    }
}

using MediatR;
using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;
using FalandoSobre.SharedKernel;
using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Api.Endpoints.Reports;

public sealed class CreateReportEndpoint : IEndpoint
{
    public sealed record Request(string ReportName, string TypeReport, string ReportDescription, string UserName, bool IsEvent, bool Actived, string ApplicationUserId);

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/reports/create", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateReportCommand(
                request.ReportName,
                request.TypeReport,
                request.ReportDescription,
                request.UserName,
                request.IsEvent,
                request.Actived,
                request.ApplicationUserId);

            Result <CreateReportResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );

        })
        .WithName("CreateReport")
        .WithTags("Reports")
        .WithOpenApi();  

    }
}

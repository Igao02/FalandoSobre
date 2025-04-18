﻿using MediatR;
using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;
using FalandoSobre.SharedKernel;

namespace FalandoSobre.Api.Endpoints.Reports;

public sealed class CreateReportEndpoint : IEndpoint
{
    public sealed record Request(string ReportName, string TypeReport, string ReportDescription, string UserName, bool IsEvent);

    //private readonly ILogger<CreateReportEndpoint> _logger;

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/reports/create", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            //_logger.LogInformation("Cheguei aqui Jesus Cristo me ajudda: {Request}");
            var command = new CreateReportCommand(
                request.ReportName,
                request.TypeReport,
                request.ReportDescription,
                request.UserName,
                request.IsEvent);

            //_logger.LogInformation("CreateReportEndpoinaaaaaaaaaaaaaaaaat: {Request}", request);

            Result<Guid> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(new { status = "success", reportId = value }),
                CustomResults.SimpleError
            );

        })
        .WithName("CreateReport")
        .WithTags("Reports")
        .WithOpenApi();  

    }
}

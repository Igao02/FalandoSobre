
using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.ReportUseCase.ListFeed;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Report;

public sealed class ListFeedEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/reports/all", async (ISender sender, CancellationToken cancellationToke) =>
        {
            var command = new ListFeedCommand();
            Result<List<ListFeedResponse>> result = await sender.Send(command, cancellationToke);
            return result.Match(
               value => Results.Ok(value),
               CustomResults.SimpleError
            );
        })
        .WithName("ListFeed")
        .WithTags("Reports");
    }
}

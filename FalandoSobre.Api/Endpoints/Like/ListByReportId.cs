using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.LikeUseCase.ListByReportId;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Like;

public sealed class ListLikesByReportIdEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/likes/report/{reportId}", async (Guid reportId, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new ListLikesByReportIdCommand(reportId);

            Result<ListLikesByReportIdResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("ListLikesByReportId")
        .WithTags("Like")
        .WithOpenApi();
    }
}

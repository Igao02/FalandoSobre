using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.CommentUseCase.ListByReportId;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Comment;

public sealed class ListCommentsByReportIdEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/comments/report/{reportId}", async (Guid reportId, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new ListCommentsByReportIdCommand(reportId);

            Result<ListCommentsByReportIdResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("ListCommentsByReportId")
        .WithTags("Comment")
        .WithOpenApi();
    }
}

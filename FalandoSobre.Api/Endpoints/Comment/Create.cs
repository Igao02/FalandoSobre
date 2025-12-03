using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.CommentUseCase.Create;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Comment;

public sealed class CreateCommentEndpoint : IEndpoint
{
    public sealed record Request(Guid ReportId, string CommentContent, string UserName, string ApplicationUserId);

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/create-comment", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateCommentCommand(
                request.ReportId,
                request.CommentContent,
                request.UserName,
                request.ApplicationUserId
            );

            Result<CommentResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("CreateComment")
        .WithTags("Comment")
        .WithOpenApi();
    }
}

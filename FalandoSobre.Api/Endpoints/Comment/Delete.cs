using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.CommentUseCase.Delete;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Comment;

public sealed class DeleteCommentEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/comments/{commentId}", async (Guid commentId, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new DeleteCommentCommand(commentId);

            Result<DeleteCommentResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("DeleteComment")
        .WithTags("Comment")
        .WithOpenApi();
    }
}

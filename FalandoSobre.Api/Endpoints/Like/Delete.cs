using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.LikeUseCase.Delete;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Like;

public sealed class DeleteLikeEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/delete-like", async (string userId, Guid reportId, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new DeleteLikeCommand(userId, reportId);

            Result<DeleteLikeResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("DeleteLike")
        .WithTags("Like")
        .WithOpenApi();
    }
}
